using System.Net;
using System.Text;
using API.Contratual.CrossCutting.Common;
using API.Contratual.CrossCutting.Notificador;
using API.Contratual.Domain.Configuracao;
using API.Contratual.Domain.Entity;
using API.Contratual.Domain.Enum;
using API.Contratual.Domain.ExtracaoTexto;
using API.Contratual.Domain.ExtracaoTexto.Payload;
using API.Contratual.Domain.Payload;
using API.Contratual.Domain.Pesquisa;
using API.Contratual.Domain.Pesquisa.Payload;
using Microsoft.Extensions.Logging;

namespace API.Contratual.Application;

public class ExtracaoTextoApplication(
    ILogger<ExtracaoTextoApplication> log,
    IExtracaoTextoService extracaoTextoService,
    IPesquisaService pesquisaService,
    IConfiguracaoService configuracaoService,
    INotificador notificador)
{
    public async Task<string> ExtrairTextoBaseadoNoDicionarioDePalavraAsync(EmpresaFilialIdsPayload ids)
{
    try
    {
        log.LogInformation("[ExtrairTextoBaseadoNoDicionarioDePalavraAsync] - Vai buscar a lista de palavras do dicionário.");
        EmpresaPalavra empresaPalavra = new EmpresaPalavra {EmpresaId = ids.EmpresaId, FilialId = ids.FilialId, Ativo = true};
        var dicionario = await pesquisaService.ObterPalavrasAsync(empresaPalavra);
        log.LogInformation("[ExtrairTextoBaseadoNoDicionarioDePalavraAsync] - Buscou a lista de palavras do dicionário.");

        log.LogInformation("[ExtrairTextoBaseadoNoDicionarioDePalavraAsync] - Vai buscar a lista de documentos.");
        var documentos = await pesquisaService.ObterTodosDocumentosAsync(ids);
        log.LogInformation("[ExtrairTextoBaseadoNoDicionarioDePalavraAsync] - Buscou a lista de documentos.");

        var processados = 0;
        var ignorados = 0;

        log.LogInformation("[ExtrairTextoBaseadoNoDicionarioDePalavraAsync] - Vai pesquisar os documentos.");
        foreach (var documento in documentos)
        {
            foreach (var palavra in dicionario)
            {
                // Verifica se já existe resultado para esta combinação de palavra e documento
                var resultadoExistente = await extracaoTextoService.BuscarResultadoPesquisaPorDicionaroIdArquivoIdAsync(
                    palavra.Id, 
                    documento.Id);

                if (resultadoExistente != null)
                {
                    log.LogInformation(
                        $"[ExtrairTextoBaseadoNoDicionarioDePalavraAsync] - Resultado já existe para palavra '{palavra.Palavra}' no documento '{documento.NomeOriginal}'");
                    ignorados++;
                    continue;
                }
                    
                var resultadoPesquisa = await pesquisaService.PesquisarAsync(palavra.Palavra, documento.Id);
                    
                if (!resultadoPesquisa.Any()) 
                    continue;
                    
                List<ResultadoPesquisaEntity> resultadoPesquisaEntities = [];
                    
                foreach (var resultadoModel in resultadoPesquisa)
                {
                    var resultadoEntity = ResultadoPesquisaEntity.ResultadoPesquisaFactory.NovoResultadoPesquisa(resultadoModel);
                    resultadoEntity.SetarDicionarioId(palavra.Id);
                    resultadoPesquisaEntities.Add(resultadoEntity);
                }
                    
                var linhasInseridas = await extracaoTextoService.InserirResultadoPesquisaAsync(resultadoPesquisaEntities);

                if (linhasInseridas > 0)
                {
                    processados += linhasInseridas;
                    log.LogInformation(
                        $"[ExtrairTextoBaseadoNoDicionarioDePalavraAsync] - Inseridos {linhasInseridas} resultados para palavra '{palavra.Palavra}' no documento '{documento.NomeOriginal}'");
                }
                else
                {
                    log.LogWarning(
                        $"[ExtrairTextoBaseadoNoDicionarioDePalavraAsync] - Falha ao inserir resultados para palavra '{palavra.Palavra}' no documento '{documento.NomeOriginal}'");
                }
            }
        }
            
        return $"Processamento finalizado. Resultados inseridos: {processados}, Combinações já existentes ignoradas: {ignorados}";
    }
    catch (Exception e)
    {
        log.LogError($"[ExtrairTextoBaseadoNoDicionarioDePalavraAsync-Erro] Erro: {e.Message}", e);
        notificador.Handle(new Notificacao(HttpStatusCode.InternalServerError));
        return $"Erro no processamento: {e.Message}";
    }
}
    
    public async Task<string> ExtrairTextoDasPaginasDosArquivosAsync(EmpresaFilialIdsPayload ids)
    {
        try
        {
            log.LogInformation("[ExtrairTextoDasPaginasDosArquivosAsync] - Vai vai obter a pasta de origem");
            var configPasta = await configuracaoService.ObterConfiguracaoPastaPorEmpresaIdFilialIdAsync(ids.EmpresaId, ids.FilialId);
            
            log.LogInformation("[ExtrairTextoDasPaginasDosArquivosAsync] - Vai buscar os arquivos nas pastas e subpastas");
            var arquivosNaPasta = extracaoTextoService.BuscarArquivosRecursivamente(configPasta.PastaOrigem);
            log.LogInformation($"[ExtrairTextoDasPaginasDosArquivosAsync] - Encontrou {arquivosNaPasta.Count} arquivos");
            
            log.LogInformation("[ExtrairTextoDasPaginasDosArquivosAsync] - Vai validar a prévia dos tamanhos");
            var (valido, excedidos) = Helper.ValidarTamanhosCaminhos(arquivosNaPasta);
            if (!valido)
            {
                var mensagemErro = new StringBuilder();
                mensagemErro.AppendLine("Processamento cancelado. Os seguintes arquivos/caminhos excedem o tamanho permitido:");
            
                foreach (var excedido in excedidos)
                {
                    var detalhes = $@"
                        Arquivo: {excedido.CaminhoCompleto}
                        Nome do arquivo: {excedido.NomeArquivo} (tamanho: {excedido.TamanhoNome}, limite: 100)
                        Caminho: {excedido.CaminhoFisico} (tamanho: {excedido.TamanhoCaminho}, limite: 250)
                        ----------------------------------------";
                
                    mensagemErro.AppendLine(detalhes);
                
                    log.LogError($"[ExtrairTextoDasPaginasDosArquivosAsync] - Arquivo excede limites: {detalhes}");
                }

                return mensagemErro.ToString();
            }
            
            
            log.LogInformation("[ExtrairTextoDasPaginasDosArquivosAsync] - Vai buscar os arquivos na pasta");
            var arquivosSelecionados = await extracaoTextoService.SelecionarArquivosParaExtrairTextoAsync(configPasta, arquivosNaPasta);
            log.LogInformation("[ExtrairTextoDasPaginasDosArquivosAsync] - Buscou os arquivos na pasta");
            
            if (arquivosSelecionados.Count == 0)
            {
                log.LogInformation("[ExtrairTextoDasPaginasDosArquivosAsync] - Nenhum arquivo novo para processar encontrado na pasta de origem");
                return "Nenhum arquivo novo para processar encontrado na pasta de origem";
            }
            
            log.LogInformation($"[ExtrairTextoDasPaginasDosArquivosAsync] - Vai fazer cópia dos arquivos: {configPasta.CriarCopia}");
            await extracaoTextoService.FazerCopiaDosArquivos(arquivosSelecionados, configPasta);
            log.LogInformation($"[ExtrairTextoDasPaginasDosArquivosAsync] - Caso config seja true, fez cópia dos arquivos");
            
            if (arquivosSelecionados.Count == 0)
            {
                log.LogInformation("[ExtrairTextoDasPaginasDosArquivosAsync] - Nenhum arquivo para processar encontrado na pasta de origem");
                return "Nenhum arquivo para processar encontrado na pasta de origem";
            }
            
            log.LogInformation("[ExtrairTextoDasPaginasDosArquivosAsync] - Vai criar registro na tabela Arquivo caso necessário");
            await extracaoTextoService.InserirArquivoAsync(arquivosSelecionados);
            log.LogInformation("[ExtrairTextoDasPaginasDosArquivosAsync] - Criou registro na tabela Arquivo caso necessário");
            
            log.LogInformation("[ExtrairTextoDasPaginasDosArquivosAsync] - Vai obter os arquivos da base para processar");
            var arquivoList = await extracaoTextoService.ObterArquivosParaProcessamentoAsync(EstadoArquivo.ProntoParaEstrairTexto);
            log.LogInformation("[ExtrairTextoDasPaginasDosArquivosAsync] - Obteve os arquivos da base para processar");
            
            log.LogInformation("[ExtrairTextoDasPaginasDosArquivosAsync] - Vai extrair os textos.");
            var resultado = await extracaoTextoService.ExtrairTextoDasPaginasDosArquivosPdfAsync(arquivoList);
            log.LogInformation("[ExtrairTextoDasPaginasDosArquivosAsync] - Extraiu os textos.");
            
            log.LogInformation($"[ExtrairTextoDasPaginasDosArquivosAsync] - Resultado: {resultado}");

            return resultado;
        }
        catch (Exception e)
        {
            log.LogError($"[ExtrairTextoDasPaginasDosArquivosAsync-Erro] - Erro ao extrair texto. Erro: {e.Message}", e);
            
            notificador.Handle(new Notificacao("Ocorreu um problema",
                "Tivemos um problema ao buscar documentos",
                TipoNotificacao.Erro,
                HttpStatusCode.InternalServerError));

            return $"Erro - {e.Message}";
        }
    }

    public async Task<string> RetentativaDeExtrairTextoDasPaginasDosArquivosAsync()
    {
        try
        {
            log.LogInformation("[RetentativaDeExtrairTextoDasPaginasDosArquivos] - Vai obter os arquivos da base para reprocessar");
            var arquivoList = await extracaoTextoService.ObterArquivosParaProcessamentoAsync(EstadoArquivo.Reprocessar);
            log.LogInformation("[RetentativaDeExtrairTextoDasPaginasDosArquivos] - Obteve os arquivos da base para reprocessar");

            if (arquivoList.Count == 0)
            {
                log.LogInformation("[RetentativaDeExtrairTextoDasPaginasDosArquivos] - Nenhum arquivo a ser reprocessado");
                return "Nenhum arquivo a ser reprocessado";
            }
        
            log.LogInformation("[RetentativaDeExtrairTextoDasPaginasDosArquivos] - Vai apagar os arquivos na tbl caso necessário");
            await extracaoTextoService.ApagarPaginasPorArquivoIdAsync(arquivoList);
            log.LogInformation("[RetentativaDeExtrairTextoDasPaginasDosArquivos] - Apagou os arquivos na tbl caso necessário");

            log.LogInformation("[RetentativaDeExtrairTextoDasPaginasDosArquivos] - Vai extrair os textos.");
            var resultado = await extracaoTextoService.ExtrairTextoDasPaginasDosArquivosPdfAsync(arquivoList);
            log.LogInformation("[RetentativaDeExtrairTextoDasPaginasDosArquivos] - Extraiu os textos.");
            
            log.LogInformation($"[RetentativaDeExtrairTextoDasPaginasDosArquivos] - Resultado: {resultado}");

            return resultado;
        }
        catch (Exception e)
        {
            log.LogError($"[RetentativaDeExtrairTextoDasPaginasDosArquivosAsync-Erro] Erro: {e.Message}", e);
            
            notificador.Handle(new Notificacao(HttpStatusCode.InternalServerError));
    
            return $"Erro no processamento: ${e.Message}";
        }
        
    }

    public async Task<string> InserirPalavrasParaPrePesquisa(PalavrasPayload palavrasPayload)
    {
        try
        {
            log.LogInformation("[InserirPalavrasParaPrePesquisa] - Vai inserir uma ou mais palavras");
            var qtdPalavrasInseridas = await extracaoTextoService.InserirPalavrasParaPrePesquisa(palavrasPayload);

            if (qtdPalavrasInseridas > 0)
            {
                notificador.Handle(new Notificacao("OK",
                    "Inserido com sucesso",
                    TipoNotificacao.Sucesso));
            }
            else
            {
                notificador.Handle(new Notificacao("Atenção",
                    "Nenhuma palavra foi cadastrada",
                    TipoNotificacao.Alerta));
            }

            return $"Inserido {qtdPalavrasInseridas} de {palavrasPayload.Palavras.Count}";
        }
        catch (Exception e)
        {
            log.LogError($"[InserirPalavrasParaPrePesquisa-Erro] Erro: {e.Message}", e);
            notificador.Handle(new Notificacao("Problema ao inserir palavra(s)",
                                                "Uma ou mais palavra(s) não foi inserida",
                                                TipoNotificacao.Erro,
                                                HttpStatusCode.InternalServerError));
            
            return $"Erro no processamento: ${e.Message}";
        }
    }
}