using API.Contratual.Application;
using API.Contratual.CrossCutting.Notificador;
using API.Contratual.Domain.ExtracaoTexto.Payload;
using API.Contratual.Domain.Payload;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace API.Contratual.WebApi.Controllers.v1;

[ApiController]
[Produces("application/json")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]/[action]")]
public class ExtracaoTextoController(ExtracaoTextoApplication extracaoTextoApplication, INotificador notificador) : BaseController(notificador)
{
    [HttpPost]
    public async Task<IActionResult> ExtrairTextoDasPaginasDosArquivosAsync([FromBody] EmpresaFilialIdsPayload empresaMaisFilial)
    {
        var processado = await extracaoTextoApplication.ExtrairTextoDasPaginasDosArquivosAsync(empresaMaisFilial);
        return CreateResponse(processado);
    }
    
    [HttpPost]
    public async Task<IActionResult> ExtrairTextoBaseadoNoDicionarioDePalavraAsync([FromBody] EmpresaFilialIdsPayload empresaMaisFilial)
    {
        var resultado = await extracaoTextoApplication.ExtrairTextoBaseadoNoDicionarioDePalavraAsync(empresaMaisFilial);
        return CreateResponse(resultado);
    }
    
    [HttpPost]
    public async Task<IActionResult> RetentativaDeExtrairTextoDasPaginasDosArquivosAsync()
    {
        var processado = await extracaoTextoApplication.RetentativaDeExtrairTextoDasPaginasDosArquivosAsync();
        return CreateResponse(processado);
    }
    
    [HttpPost]
    public async Task<IActionResult> InserirPalavrasParaPrePesquisa([FromBody] PalavrasPayload palavras)
    {
        var resultado = await extracaoTextoApplication.InserirPalavrasParaPrePesquisa(palavras);
        return CreateResponse(resultado);
    }
}