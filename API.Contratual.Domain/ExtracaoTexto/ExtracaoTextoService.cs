using System.Drawing;
using API.Contratual.CrossCutting.Common;
using API.Contratual.Domain.Empresa;
using API.Contratual.Domain.Entity;
using API.Contratual.Domain.Enum;
using API.Contratual.Domain.ExtracaoTexto.Payload;
using API.Contratual.Domain.Interface.Repository;
using API.Contratual.Dto;
using API.Contratual.Dto.Configuracao;
using Ghostscript.NET.Rasterizer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tesseract;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace API.Contratual.Domain.ExtracaoTexto;

public class ExtracaoTextoService(
    IDocumentoRepository documentoRepository, 
    ILogger<ExtracaoTextoService> log, 
    IEmpresaService empresaService,
    IOptions<AppSettings> appSettings) : IExtracaoTextoService
{
    public List<string> BuscarArquivosRecursivamente(string pasta)
    {
        List<string> arquivosEncontrados = [];
        try 
        {
            // Adiciona arquivos do diretório atual
            arquivosEncontrados.AddRange(Directory.GetFiles(pasta));
            
            // Busca recursivamente em todos os subdiretórios
            foreach (var subdiretorio in Directory.GetDirectories(pasta))
            {
                arquivosEncontrados.AddRange(BuscarArquivosRecursivamente(subdiretorio));
            }
        }
        catch (Exception ex)
        {
            log.LogError($"[BuscarArquivosRecursivamente] Erro ao acessar pasta {pasta}: {ex.Message}");
        }
        return arquivosEncontrados;
    }
    
    public async Task<int> InserirResultadoPesquisaAsync(List<ResultadoPesquisaEntity> resultadoPesquisaEntities)
    {
        var linhasInseridas = await documentoRepository.InserirResultadoPesquisaAsync(resultadoPesquisaEntities);
        return linhasInseridas;
    }

    public async Task<ResultadoPesquisaEntity?> BuscarResultadoPesquisaPorDicionaroIdArquivoIdAsync(Guid dicionarioId, Guid arquivoId)
    {
        return await documentoRepository.BuscarResultadoPesquisaPorDicionaroIdArquivoIdAsync(dicionarioId, arquivoId);
    }
    
    public async Task<List<ArquivoEntity>> SelecionarArquivosParaExtrairTextoAsync(ConfiguracaoPastaDto configPasta, List<string> arquivosNaPasta)
    {
        List<ArquivoEntity> arquivos = [];

        foreach (var arquivoNaPasta in arquivosNaPasta)
        {
            var nomeDoArquivo = Path.GetFileName(arquivoNaPasta);
            var caminhoFisico = Path.GetDirectoryName(arquivoNaPasta);
            var extensaoDoArquivo = Path.GetExtension(arquivoNaPasta).Replace(".", string.Empty);
            var fileInfo = new FileInfo(arquivoNaPasta);
        
            log.LogInformation($"[SelecionarArquivosParaExtrairTextoAsync] - Processando arquivo: {nomeDoArquivo} ({fileInfo.Length} bytes)");
        
            var arquivoExistente = await documentoRepository.BuscarArquivoProcessadoPorIdentificacao(
                nomeDoArquivo, 
                fileInfo.Length);

            if (arquivoExistente?.EstadoArquivoId == EstadoArquivo.TextoDaPaginaExtraido)
            {
                continue;
            }

            if (arquivoExistente == null)
            {
                var arquivo = ArquivoEntity.ArquivoFactory.NovoArquivo(
                    configPasta.EmpresaId, 
                    configPasta.FilialId, 
                    nomeDoArquivo, 
                    fileInfo.Length, 
                    configPasta.CriarCopia
                );
            
                arquivo.SetarNomeOriginalDoArquivo(nomeDoArquivo);
                arquivo.SetarLocalDoArquivo(caminhoFisico);
                arquivo.SetarTipoDoArquivo(extensaoDoArquivo);
                arquivos.Add(arquivo);
            }
        }

        return arquivos;
    }

    public async Task ApagarPaginasPorArquivoIdAsync(List<ArquivoEntity> arquivoList)
    {
        foreach (var arquivo in arquivoList)
        {
            var apagado = await documentoRepository.ApagarArquivoPaginaPorArquivoIdAsync(arquivo.Id);

            if (!apagado)
            {
                log.LogError(
                    $"[ApagarPaginasPorArquivoIdAsync] - Erro ao apagar processamento do {arquivo.NomeOriginal}");

                List<ArquivoEntity> arquivos = [arquivo];
                await documentoRepository.AtualizarArquivosAsync(arquivos);
            }
        }
    }

    public async Task<int> InserirArquivoAsync(List<ArquivoEntity> arquivoList)
    {
        if (arquivoList.Count == 0)
        {
            return 0;
        }

        List<ArquivoEntity> arquivosNovos = [];

        foreach (var arquivo in arquivoList)
        {
            if (arquivo.EstadoArquivoId is EstadoArquivo.ProntoParaEstrairTexto)
            {
                arquivosNovos.Add(arquivo);
            }
        }

        if (arquivosNovos.Count <= 0) return 0;

        var linhasInseridas = await documentoRepository.InserirArquivoAsync(arquivosNovos);

        if (linhasInseridas == 0)
        {
            log.LogError("[InserirArquivoAsync] - Erro ao inserir Arquivo");
        }
        
        return linhasInseridas;
    }

    public async Task<string> ExtrairTextoDasPaginasDosArquivosPdfAsync(List<ArquivoEntity> arquivoList)
    {
        var arquivoPdfList = arquivoList.Where(arquivo => arquivo.NomeOriginal.ToLower().EndsWith(".pdf")).ToList();
        
        var arquivosProcessados = 0;

        foreach (var arquivo in arquivoPdfList)
        {
            arquivo.SetarEstadoArquivo(EstadoArquivo.EmAndamento);
            arquivo.SetarDataProcessamento();
            await documentoRepository.AtualizarArquivoAsync(arquivo);
            
            var textoExtraidoList = new List<ArquivoPaginaEntity>();
            var diretorioDeOrigemMaisArquivo = Path.Combine(arquivo.CaminhoFisico, arquivo.NomeOriginal);
            
            //letura do arquivo para processar via OCR
            byte[] buffer = await File.ReadAllBytesAsync(diretorioDeOrigemMaisArquivo);
            using var ms = new MemoryStream(buffer);
            
            //letura do arquivo para processar via PdfPig
            using var document = PdfDocument.Open(@$"{diretorioDeOrigemMaisArquivo}");
            
            for (var i = 0; i < document.NumberOfPages; i++)
            {
                int numPaginaAtual = i + 1;
                var pagina = document.GetPage(numPaginaAtual);
                string texto;

                // Tenta extrair o texto normalmente
                texto = ContentOrderTextExtractor.GetText(pagina);

                // Se o texto estiver vazio ou for muito curto, tenta OCR
                if (string.IsNullOrWhiteSpace(texto) || texto.Length < 50)
                {
                    int dpi = 300;
                    using var rasterizer = new GhostscriptRasterizer();
                    rasterizer.Open(ms);
                    Image img = rasterizer.GetPage(dpi, numPaginaAtual);
                    using var engine = new TesseractEngine(@"./tessdata", "por", EngineMode.Default);
                    using var image = new Bitmap(img);
                    using var pix = PixConverter.ToPix(image);
                    using var page = engine.Process(pix);
                    texto = page.GetText();
                }
                
                if(string.IsNullOrWhiteSpace(texto))
                    continue;

                var arquivoPagina = ArquivoPaginaEntity.ArquivoPaginaFactory.NovoArquivo(arquivo.Id, (short)numPaginaAtual, texto);

                textoExtraidoList.Add(arquivoPagina);
            }

            var linhasInseridas = await documentoRepository.InserirPaginaDocumento(textoExtraidoList);
            
            if (linhasInseridas != textoExtraidoList.Count)
            {
                log.LogError(
                    $"[ExtrairTextoDosArquivosAsync-Erro] - Erro ao inserir páginas: arquivo {arquivo.NomeOriginal}, qtd de páginas inseridas {linhasInseridas}, qtd total {textoExtraidoList.Count}");
                arquivo.SetarEstadoArquivo(EstadoArquivo.Reprocessar);
            }
      
            arquivo.SetarEstadoArquivo(EstadoArquivo.TextoDaPaginaExtraido);
            
            await documentoRepository.AtualizarArquivoAsync(arquivo);
            
            arquivosProcessados += 1;
        }
        
        return $"{arquivosProcessados} arquivos processados";
    }
    
    public async Task FazerCopiaDosArquivos(List<ArquivoEntity> arquivoList, ConfiguracaoPastaDto configPasta)
    {
        if (configPasta.CriarCopia)
        {
            var empresa = await empresaService.ObterEmpresaPorIdAsync(configPasta.EmpresaId);
            var filial = await empresaService.ObterFilialPorIdAsync(configPasta.FilialId);

            // Formata o nome da pasta da empresa
            var razaoSocial = Helper.FormatarNomePasta(empresa.RazaoSocial);
            var pastaEmpresa = razaoSocial + "_" + empresa.Cnpj;

            // Formata o nome da pasta da filial
            var pastaFilial = Helper.FormatarNomePasta(filial.Nome);

            // Combina os caminhos: pasta destino/empresa/filial
            var diretorioDeDestino = Path.Combine(configPasta.PastaDestino, pastaEmpresa, pastaFilial);

            // Cria a estrutura de diretórios completa
            Directory.CreateDirectory(diretorioDeDestino);

            foreach (var arquivo in arquivoList)
            {
                if(arquivo.EstadoArquivoId != EstadoArquivo.ProntoParaEstrairTexto)
                    continue;
                
                var diretorioDeDestinoMaisArquivo = Path.Combine(diretorioDeDestino, arquivo.NomeCopia);
                var diretorioDeOrigemMaisArquivo = Path.Combine(arquivo.CaminhoFisico, arquivo.NomeOriginal);
                File.Copy(diretorioDeOrigemMaisArquivo, diretorioDeDestinoMaisArquivo);
            }
        }
    }

    public async Task<List<ArquivoEntity>> ObterArquivosParaProcessamentoAsync(EstadoArquivo status)
    {
        var arquivoEntity = await documentoRepository.ObterArquivoPorStatusAsync(EstadoArquivo.ProntoParaEstrairTexto);
        return (arquivoEntity ?? Array.Empty<ArquivoEntity>()).ToList();
    }
    
    public async Task<int> InserirPalavrasParaPrePesquisa(PalavrasPayload palavrasPayload)
    {
        if (palavrasPayload.Palavras.Count == 0)
            return 0;
        
        var dicionarioList = new List<DicionarioEntity>();
        
        foreach (var palavra in palavrasPayload.Palavras)
        {
            var dicionario = DicionarioEntity.DicionarioEntiryFactory.Novo(palavra, true, palavrasPayload.EmpresaId, palavrasPayload.FilialId);
            dicionarioList.Add(dicionario);
        }
        
        var linhasInseridas = await documentoRepository.InserirPalavraDicionarioAsync(dicionarioList);

        if (linhasInseridas == 0)
        {
            log.LogError("[InserirPalavrasParaPrePesquisa-erro] - Erro ao inserir Arquivo");
        }
        
        return linhasInseridas;
    }
}