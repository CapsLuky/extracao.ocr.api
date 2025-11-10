using API.Contratual.Application;
using API.Contratual.CrossCutting.Notificador;
using API.Contratual.Domain.Configuracao.Payload;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace API.Contratual.WebApi.Controllers.v1;

[ApiController]
[Produces("application/json")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]/[action]")]
public class ConfiguracaoController(ConfiguracaoApplication configuracaoApplication, INotificador notificador) : BaseController(notificador)
{
    
    [HttpPost]
    public async Task<IActionResult> ConfiguracaoPastaAsync([FromBody] ConfiguracaoPastaIdsPayload configuracaoPastaIds)
    {
        var processado = await configuracaoApplication.InserirConfiguracaoPastaAsync(configuracaoPastaIds);
        return CreateResponse(processado);
    }
    
    [HttpPatch]
    public async Task<IActionResult> ConfiguracaoPastaAsync([FromQuery] Guid id, [FromBody] ConfiguracaoPastaPatchPayload configuracaoPasta)
    {
        var processado = await configuracaoApplication.AtualizarConfiguracaoPastaAsync(id, configuracaoPasta);
        return CreateResponse(processado);
    }
    
    [HttpGet]
    public async Task<IActionResult> ConfiguracaoPastaPorIdAsync([FromQuery] Guid id)
    {
        var res = await configuracaoApplication.ObterConfiguracaoPastaPorIdAsync(id);
        return CreateResponse(res);
    }
    
    [HttpPost]
    public async Task<IActionResult> ConfiguracaoAsync([FromBody] ConfiguracaoPaylod configuracao)
    {
        var processado = await configuracaoApplication.InserirConfiguracaoAsync(configuracao);
        return CreateResponse(processado);
    }
    
    [HttpPatch]
    public async Task<IActionResult> ConfiguracaoAsync([FromQuery] Guid id, [FromBody] ConfiguracaoPatchPayload configuracao)
    {
        var processado = await configuracaoApplication.AtualizarEmpresaAsync(id, configuracao);
        return CreateResponse(processado);
    }

    [HttpGet]
    public async Task<IActionResult> ConfiguracaoPorIdAsync([FromQuery] Guid id)
    {
        var res = await configuracaoApplication.ObterConfiguracaoPorIdAsync(id);
        return CreateResponse(res);
    }
    
    [HttpGet]
    public async Task<IActionResult> ConfiguracaoPorEmpresaIdAsync([FromQuery] Guid empresaId)
    {
        var res = await configuracaoApplication.ObterConfiguracaoPorEmpresaIdAsync(empresaId);
        return CreateResponse(res);
    }
}