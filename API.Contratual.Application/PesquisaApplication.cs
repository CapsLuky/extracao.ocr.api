using System.Net;
using API.Contratual.CrossCutting.Notificador;
using API.Contratual.Domain.Configuracao;
using API.Contratual.Domain.Entity;
using API.Contratual.Domain.Payload;
using API.Contratual.Domain.Pesquisa;
using API.Contratual.Domain.Pesquisa.Payload;
using API.Contratual.Dto.Pesquisa;
using Microsoft.Extensions.Logging;

namespace API.Contratual.Application;

public class PesquisaApplication(ILogger<PesquisaApplication> log,
    IPesquisaService pesquisaService,
    IConfiguracaoService configuracaoService,
    INotificador notificador)
{
    public async Task<IEnumerable<ArquivoEntity>> ObterTodosDocumentosAsync(EmpresaFilialIdsPayload ids)
    {
        try
        {
            log.LogInformation("[BuscarDocumentosAsync] - Vai obter os arquivos");
            IEnumerable<ArquivoEntity> arquivoList = await pesquisaService.ObterTodosDocumentosAsync(ids);
            return arquivoList;
        }
        catch (Exception e)
        {
            log.LogError($"[BuscarDocumentosAsync-Erro] - Erro ao buscar documentos: {e.Message}", e);
            
            notificador.Handle(new Notificacao("Ocorreu um problema",
                "Tivemos um problema ao buscar documentos",
                TipoNotificacao.Erro,
                HttpStatusCode.InternalServerError));
            
            return [];
        }
    }
    
    public async Task<IEnumerable<PaginaDocumentoDto>> PesquisarAsync(string palavras, Guid arquivoId)
    {
        try
        {
            log.LogInformation("[PesquisarAsync] - Irá pesquisar os documentos.");
            
            var documentosView = await pesquisaService.PesquisarAsync(palavras, arquivoId);

            if (!documentosView.Any())
            {
                notificador.Handle(new Notificacao("Pesquisa",
                    "Nenhuma ocorrência da(s) palavra(s) encontrada",
                    TipoNotificacao.Informacao));
            }
            
            log.LogInformation("[PesquisarAsync] - Pesquisou os documentos.");

            return documentosView;
        }
        catch (Exception e)
        {
            log.LogError($"[PesquisarAsync-Erro] - Erro ao pesquisar palavras. Erro: {e.Message}", e);
            
            notificador.Handle(new Notificacao("Ocorreu um problema",
                "Tivemos um problema ao buscar documentos",
                TipoNotificacao.Erro,
                HttpStatusCode.InternalServerError));

            return [];
        }
    }
    
    public async Task<IEnumerable<PaginaDocumentoDto>> BuscarResultadoPesquisaAsync(BuscarResultadoPesquisaPayload payload)
    {
        try
        {
            log.LogInformation("[BuscarResultadoPesquisaAsync] - Irá buscar pesquisas já processadas.");
            
            var documentosModel = await pesquisaService.BuscarResultadoPesquisaAsync(payload);
            
            var documentosView = documentosModel.Select(d => (PaginaDocumentoDto)d).ToList();

            return documentosView;
        }
        catch (Exception e)
        {
            log.LogError($"[BuscarResultadoPesquisaAsync-Erro] - Erro ao pesquisar palavras. Erro: {e.Message}", e);
            
            notificador.Handle(new Notificacao("Ocorreu um problema",
                "Tivemos um problema ao buscar documentos",
                TipoNotificacao.Erro,
                HttpStatusCode.InternalServerError));

            return [];
        }
    }
    
    public async Task<(byte[], HttpStatusCode, string)> BuscarArquivoPdfPorIdAsync(ObterArquivoPayload payload)
    {
        try
        {
            var config = await configuracaoService.ObterConfiguracaoPastaPorEmpresaIdFilialIdAsync(payload.EmpresaId, payload.FilialId);
            var resultado = await pesquisaService.BuscarArquivoPdfPorIdAsync(payload.ArquivoId, config.CriarCopia);

            if (resultado.Item2 != HttpStatusCode.OK)
                notificador.Handle(new Notificacao("Arquivo não encontrado",
                    "Não encontramos este arquivo no servidor.",
                    TipoNotificacao.Erro,
                    resultado.Item2));

            return resultado;

        }
        catch (Exception e)
        {
            log.LogError($"[BuscarArquivoPdfPorIdAsync-Erro] - Erro ao buscar arquivo: {e.Message}", e);
            
            notificador.Handle(new Notificacao("Ocorreu um problema",
                "Tivemos um problema ao buscar documentos",
                TipoNotificacao.Erro,
                HttpStatusCode.InternalServerError));

            return ([], HttpStatusCode.InternalServerError, String.Empty);
        }
    }
    
    public async Task<IEnumerable<DicionarioEntity>> ObterPalavrasAsync(EmpresaPalavra empresaPalavra)
    {
        try
        {
            IEnumerable<DicionarioEntity> palavraList = await pesquisaService.ObterPalavrasAsync(empresaPalavra);
            return palavraList;
        }
        catch (Exception e)
        {
            log.LogError($"[ObterPalavrasAsync-Erro] - Erro ao buscar palavras do diciário: {e.Message}", e);
            
            notificador.Handle(new Notificacao("Ocorreu um problema",
                "Tivemos um problema ao buscar as palavras usadas nas pesquisas",
                TipoNotificacao.Erro,
                HttpStatusCode.InternalServerError));
            
            return [];
        }
    }

    public async Task<bool> AtualizarValidadeResultadoPesquisaAsync(Guid id, bool ativo)
    {
        try
        {
            log.LogInformation($"[AtualizarValidadeResultadoPesquisaAsync] - Vai atualizar a tabela resultadopesquisa. Id: {id}, Resultado: {ativo}");
            var result = await pesquisaService.AtualizarValidadeResultadoPesquisaAsync(id, ativo);
            log.LogInformation($"[AtualizarValidadeResultadoPesquisaAsync] - Atualizou a tabela resultadopesquisa, resultado: {result}");

            if (result == 0)
            {
                notificador.Handle(new Notificacao("Ocorreu um problema",
                    "Tivemos um problema ao atualizar o resultado da pesquisa",
                    TipoNotificacao.Erro,
                    HttpStatusCode.InternalServerError));
                
                return false;
            }

            notificador.Handle(new Notificacao(string.Empty, "Registro arquivado com sucesso", TipoNotificacao.Sucesso));
            
            return true;
        }
        catch (Exception e)
        {
            log.LogError($"[AtualizarValidadeResultadoPesquisaAsync-Erro] - Erro ao atualizar resultado da pesquisa: {e.Message}", e);
            
            notificador.Handle(new Notificacao("Ocorreu um problema",
                "Tivemos um problema ao atualizar o resultado da pesquisa",
                TipoNotificacao.Erro,
                HttpStatusCode.InternalServerError));

            return false;
        }
    }

    public async Task<byte[]?> ObterImageDaPaginaAsync(ObterImagemPaginaPayload payload)
    {
        try
        {
            var config = await configuracaoService.ObterConfiguracaoPastaPorEmpresaIdFilialIdAsync(payload.EmpresaId, payload.FilialId);

            log.LogInformation($"[ObterImageDaPaginaAsync] - Vai buscar o PDF. arquivoId: {payload.ArquivoId}, numPagina: {payload.NumeroPagina}");
            var retornoBusca = await pesquisaService.BuscarArquivoPdfPorIdAsync(payload.ArquivoId, config.CriarCopia);
            log.LogInformation($"[ObterImageDaPaginaAsync] - Buscou o PDF. statusCode: {retornoBusca.Item2}, nome do arquivo: {retornoBusca.Item3}");
            
            var retornoHttpStatusCode = retornoBusca.Item2;

            if (retornoHttpStatusCode != HttpStatusCode.OK)
            {
                notificador.Handle(new Notificacao("Página não encontrada",
                    "Ocorreu um problema ao buscar o arquivo",
                    TipoNotificacao.Erro,
                    retornoHttpStatusCode));
            
                return null;
            }
            
            log.LogInformation($"[ObterImageDaPaginaAsync] - Vai obter a imagem a partir do PDF. arquivoId: {payload.ArquivoId}, numPagina: {payload.NumeroPagina}");
            var byteImagem = pesquisaService.ObterImagemDoPdfAsync(retornoBusca.Item1, payload.NumeroPagina);
            log.LogInformation($"[ObterImageDaPaginaAsync] - Obteve a imagem a partir do PDF. arquivoId: {payload.ArquivoId}, numPagina: {payload.NumeroPagina}");
            
            return byteImagem;
        }
        catch (Exception e)
        {
            log.LogError($"[ObterImageDaPaginaAsync-Erro] - Erro ao buscar imagem da página: {e.Message}", e);
            
            notificador.Handle(new Notificacao("Ocorreu um problema",
                "Tivemos um problema para exibir a página",
                TipoNotificacao.Erro,
                HttpStatusCode.InternalServerError));

            return null;
        }
    }
}