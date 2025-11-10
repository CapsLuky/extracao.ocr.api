using API.Contratual.Application;
using API.Contratual.CrossCutting.Notificador;
using API.Contratual.Domain.Empresa.Payload;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace API.Contratual.WebApi.Controllers.v1;

[ApiController]
[Produces("application/json")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]/[action]")]
public class EmpresaController(EmpresaApplication extracaoTextoApplication, INotificador notificador) : BaseController(notificador)
{
    [HttpPost]
    public async Task<IActionResult> EmpresaAsync([FromBody] EmpresaPayload empresa)
    {
        var result = await extracaoTextoApplication.InserirEmpresaAsync(empresa);
        return CreateResponse(result);
    }
    
    [HttpPatch]
    public async Task<IActionResult> EmpresaAsync(
        [FromQuery] Guid id,
        [FromBody] EmpresaPayload empresa)
    {
        var result = await extracaoTextoApplication.AtualizarEmpresaAsync(id, empresa);
        return CreateResponse(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> ObterEmpresaPorIdAsync([FromQuery] Guid id)
    {
        var result = await extracaoTextoApplication.ObterEmpresaPorIdAsync(id);
        return CreateResponse(result);
    }

    [HttpGet]
    public async Task<IActionResult> ObterEmpresaPorCnpjAsync([FromQuery] string cnpj)
    {
        var result = await extracaoTextoApplication.ObterEmpresaPorCnpjAsync(cnpj);
        return CreateResponse(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> FilialAsync([FromBody] FilialPayload filial)
    {
        var result = await extracaoTextoApplication.InserirFilialAsync(filial);
        return CreateResponse(result);
    }
    
    [HttpPatch]
    public async Task<IActionResult> FilialAsync(
        [FromQuery] Guid id,
        [FromBody] FilialPayload filial)
    {
        var result = await extracaoTextoApplication.AtualizarFilialAsync(id, filial);
        return CreateResponse(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> ObterFilialPorIdAsync([FromQuery] Guid id)
    {
        var result = await extracaoTextoApplication.ObterFilialPorIdAsync(id);
        return CreateResponse(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> ObterFiliaisPorEmpresaIdAsync([FromQuery] Guid id)
    {
        var result = await extracaoTextoApplication.ObterFiliaisPorEmpresaIdAsync(id);
        return CreateResponse(result);
    }
}