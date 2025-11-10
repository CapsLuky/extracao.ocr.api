using API.Contratual.Application;
using API.Contratual.CrossCutting.Notificador;
using API.Contratual.Domain.Payload;
using API.Contratual.Domain.Pesquisa.Payload;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace API.Contratual.WebApi.Controllers.v1;

[ApiController]
[Produces("application/json")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]/[action]")]
public class PesquisaController : BaseController
{
    private readonly PesquisaApplication _pesquisaApplication;

    public PesquisaController(PesquisaApplication pesquisaApplication, INotificador notificador) :
        base(notificador)
    {
        _pesquisaApplication = pesquisaApplication;
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodosDocumentosAsync([FromQuery] EmpresaFilialIdsPayload ids)
    {
        var documentos = await _pesquisaApplication.ObterTodosDocumentosAsync(ids);
        return CreateResponse(documentos);
    }

    [HttpGet("palavras")]
    public async Task<IActionResult> PesquisarAsync(string palavras, Guid arquivoId)
    {
        var documentos = await _pesquisaApplication.PesquisarAsync(palavras, arquivoId);

        return CreateResponse(documentos);
    }

    [HttpGet]
    public async Task<IActionResult> BuscarResultadoPesquisaAsync([FromQuery] BuscarResultadoPesquisaPayload payload)
    {
        var documentos = await _pesquisaApplication.BuscarResultadoPesquisaAsync(payload);
        return CreateResponse(documentos);
    }
    
    [HttpGet]
    public async Task<IActionResult> ObterArquivoAsync([FromQuery] ObterArquivoPayload payload)
    {
        var documento = await _pesquisaApplication.BuscarArquivoPdfPorIdAsync(payload);

        if (documento.Item1.Length > 0)
        {
            return File(documento.Item1, "application/pdf", documento.Item3);
        }
        
        return BadRequest();
    }

    [HttpGet]
    public async Task<IActionResult> ObterPalavrasAsync([FromQuery] EmpresaPalavra empresaPalavra)
    {
        var palavraList = await _pesquisaApplication.ObterPalavrasAsync(empresaPalavra);
        return CreateResponse(palavraList);
    }

    [HttpPatch]
    public async Task<IActionResult> ArquivarResultadoPesquisaAsync(Guid id, bool resultadoInvalido)
    {
        var result = await _pesquisaApplication.AtualizarValidadeResultadoPesquisaAsync(id, resultadoInvalido);
        return CreateResponse(result);
    }

    [HttpGet]
    public async Task<IActionResult> ObterImagemPaginaAsync([FromQuery] ObterImagemPaginaPayload payload)
    {
        var arquivo = await _pesquisaApplication.ObterImageDaPaginaAsync(payload);
        
        if (arquivo == null || arquivo.Length == 0)
        {
            // Usa o padrão de notificação que já está sendo utilizado no PesquisaApplication
            // O notificador já deve ter sido alimentado com a mensagem de erro apropriada
            return CreateResponse(null);
        }
        
        return File(arquivo, "image/jpeg");
    }
}
