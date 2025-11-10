using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using API.Contratual.Domain.Entity;
using API.Contratual.Domain.Enum;
using API.Contratual.Domain.ExtracaoTexto;
using API.Contratual.Domain.Interface.Repository;
using API.Contratual.Domain.Payload;
using API.Contratual.Domain.Pesquisa.Model;
using API.Contratual.Domain.Pesquisa.Payload;
using API.Contratual.Dto;
using API.Contratual.Dto.Pesquisa;
using Ghostscript.NET.Rasterizer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace API.Contratual.Domain.Pesquisa;

public class PesquisaService(
    IDocumentoRepository documentoRepository,
    ILogger<ExtracaoTextoService> log,
    IOptions<AppSettings> appSettings) : IPesquisaService
{
    public async Task<IEnumerable<ArquivoEntity>> ObterTodosDocumentosAsync(EmpresaFilialIdsPayload ids)
    {
        IEnumerable<ArquivoEntity> arquivoEnity = await documentoRepository.ObterTodosDocumentosAsync(ids);
        return arquivoEnity;
    }

    public async Task<IEnumerable<PaginaDocumentoModel>> BuscarResultadoPesquisaAsync(BuscarResultadoPesquisaPayload payload)
    {
        var documentosEntity = await documentoRepository.BuscarResultadoPesquisaListAsync(payload);
        var documentos = documentosEntity.Select(d => (PaginaDocumentoModel)d).ToList();

        return documentos.Select(d => (PaginaDocumentoModel)d);
    }
    
    public async Task<IEnumerable<PaginaDocumentoDto>> PesquisarAsync(string textos, Guid arquivoId)
    {
        var documentosModel =
            await documentoRepository.PesquisarPalavrasAsync(textos, arquivoId, ModoDeBusca.NaturalLanguageMode);

        foreach (var documento in documentosModel)
        {
            string[] palavrasChave = textos.Split(' ');
            documento.ExtrairTrecho(palavrasChave[0]);
        }

        return documentosModel.Select(d => (PaginaDocumentoDto)d);
    }

    public async Task<(byte[], HttpStatusCode, string)> BuscarArquivoPdfPorIdAsync(Guid arquivoId, bool temCopia)
    {
        var result = await documentoRepository.ObterArquivoPorIdAsync(arquivoId);
        var filePath = "";
        
        if (result == null)
        {
            return ([], HttpStatusCode.NotFound, string.Empty);
        }

        filePath = Path.Combine(result.CaminhoFisico, temCopia ? result.NomeCopia : result.NomeOriginal);
        
        if (!File.Exists(filePath))
        {
            return ([], HttpStatusCode.NotFound, string.Empty);
        }

        byte[] fileBytes = await File.ReadAllBytesAsync(filePath);

        return (fileBytes, HttpStatusCode.OK, result.NomeCopia);
    }

    public async Task<IEnumerable<DicionarioEntity>> ObterPalavrasAsync(EmpresaPalavra empresaPalavra)
    {
        IEnumerable<DicionarioEntity> palavraList = await documentoRepository.ObterPalavrasAsync(empresaPalavra);
        return palavraList;
    }

    public async Task<int> AtualizarValidadeResultadoPesquisaAsync(Guid id, bool ativo)
    {
        return await documentoRepository.AtualizarValidadeResultadoPesquisaAsync(id, ativo);
    }

    public byte[] ObterImagemDoPdfAsync(byte[] arquivoPdf, int numeroPagina)
    {
        int dpi = 300;

        using var ms = new MemoryStream(arquivoPdf);
        using var rasterizer = new GhostscriptRasterizer();
        rasterizer.Open(ms);
        Image img = rasterizer.GetPage(dpi, numeroPagina);

        var memoryStream = new MemoryStream();
        img.Save(memoryStream, ImageFormat.Jpeg);
        memoryStream.Seek(0, SeekOrigin.Begin);

        return memoryStream.ToArray();
    }
}