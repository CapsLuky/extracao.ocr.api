using System.Net;
using API.Contratual.CrossCutting.Notificador;
using API.Contratual.Domain.Configuracao;
using API.Contratual.Domain.Configuracao.Payload;
using API.Contratual.Dto.Configuracao;
using Microsoft.Extensions.Logging;

namespace API.Contratual.Application;

public class ConfiguracaoApplication(
    ILogger<ConfiguracaoApplication> log, 
    IConfiguracaoService configuracaoService, 
    INotificador notificador)
{
    #region Configuração pasta

    public async Task<ConfiguracaoPastaDto?> InserirConfiguracaoPastaAsync(ConfiguracaoPastaIdsPayload configuracaoPastaIds)
    {
        try
        {
            var configuracaoPastaVm = await configuracaoService.InserirConfiguracaoPastaAsync(configuracaoPastaIds);

            if (configuracaoPastaVm == null && !Guid.TryParse(configuracaoPastaVm.Id.ToString(), out Guid _))
            {
                log.LogError($"[InserirConfiguracaoPastaAsync-Erro] Erro ao inserir configuração de pasta: Filial {configuracaoPastaVm.FilialId}");
                
                if (notificador.ObterNotificacoes().Count != 0) return null;
                
                notificador.Handle(new Notificacao("Ocorreu um problema",
                    "Tivemos um problema para criar configuração de pasta",
                    TipoNotificacao.Erro,
                    HttpStatusCode.InternalServerError));
                
                return null;
            }
            
            return configuracaoPastaVm;
        }
        catch (Exception e)
        {
            log.LogError($"[InserirConfiguracaoPastaAsync-Erro] Erro: {e.Message}", e);
            notificador.Handle(new Notificacao("Ocorreu um problema",
                "Tivemos um problema para criar configuração de pasta",
                TipoNotificacao.Erro,
                HttpStatusCode.InternalServerError));
            return null;
        }
    }
    
    public async Task<ConfiguracaoPastaDto?> AtualizarConfiguracaoPastaAsync(Guid id, ConfiguracaoPastaPatchPayload configuracaoPasta)
    {
        try
        {
            return await configuracaoService.AtualizarConfiguracaoPastaAsync(id, configuracaoPasta);
        }
        catch (Exception e)
        {
            log.LogError($"[AtualizarConfiguracaoPastaAsync-Erro] Erro: {e.Message}", e);
            notificador.Handle(new Notificacao("Ocorreu um problema",
                "Tivemos um problema para atualizar a configuração de pasta",
                TipoNotificacao.Erro,
                HttpStatusCode.InternalServerError));
            return null;
        }
    }
    
    public async Task<ConfiguracaoPastaDto?> ObterConfiguracaoPastaPorIdAsync(Guid id)
    {
        try
        {
            return await configuracaoService.ObterConfiguracaoPastaPorIdAsync(id);
        }
        catch (Exception e)
        {
            log.LogError($"[ObterConfiguracaoPastaPorIdAsync-Erro] Erro: {e.Message}", e);
            notificador.Handle(new Notificacao("Ocorreu um problema",
                "Tivemos um problema para buscar a configuração de pasta",
                TipoNotificacao.Erro,
                HttpStatusCode.InternalServerError));
            return null;
        }
    }
    
    #endregion
    
    #region Configuração
    
    public async Task<ConfiguracaoDto?> InserirConfiguracaoAsync(ConfiguracaoPaylod configuracao)
    {
        try
        {
            var configuracaoVm = await configuracaoService.InserirConfiguracaoAsync(configuracao);

            if (configuracaoVm == null && !Guid.TryParse(configuracaoVm.Id.ToString(), out Guid _))
            {
                log.LogError($"[InserirConfiguracaoAsync-Erro] Erro ao inserir configuração: Filial {configuracaoVm.FilialId}");
                
                if (notificador.ObterNotificacoes().Count != 0) return null;
                
                notificador.Handle(new Notificacao("Ocorreu um problema",
                    "Tivemos um problema para criar configuração",
                    TipoNotificacao.Erro,
                    HttpStatusCode.InternalServerError));
                
                return null;
            }
            
            return configuracaoVm;
        }
        catch (Exception e)
        {
            log.LogError($"[InserirConfiguracaoAsync-Erro] Erro: {e.Message}", e);
            notificador.Handle(new Notificacao("Ocorreu um problema",
                "Tivemos um problema para criar configuração",
                TipoNotificacao.Erro,
                HttpStatusCode.InternalServerError));
            return null;
        }
    }
    
    public async Task<ConfiguracaoDto?> AtualizarEmpresaAsync(Guid id, ConfiguracaoPatchPayload configuracao)
    {
        try
        {
            return await configuracaoService.AtualizarConfiguracaoAsync(id, configuracao);
        }
        catch (Exception e)
        {
            log.LogError($"[AtualizarEmpresaAsync-Erro] Erro: {e.Message}", e);
            notificador.Handle(new Notificacao("Ocorreu um problema",
                "Tivemos um problema para atualizar a configuração",
                TipoNotificacao.Erro,
                HttpStatusCode.InternalServerError));
            return null;
        }
    }

    public async Task<ConfiguracaoDto?> ObterConfiguracaoPorIdAsync(Guid id)
    {
        try
        {
            return await configuracaoService.ObterConfiguracaoPorIdAsync(id);
        }
        catch (Exception e)
        {
            log.LogError($"[ObterConfiguracaoPorIdAsync-Erro] Erro: {e.Message}", e);
            notificador.Handle(new Notificacao("Ocorreu um problema",
                "Tivemos um problema para buscar a configuração",
                TipoNotificacao.Erro,
                HttpStatusCode.InternalServerError));
            return null;
        }
    }
    
    public async Task<List<ConfiguracaoDto>> ObterConfiguracaoPorEmpresaIdAsync(Guid empresaId)
    {
        try
        {
            return await configuracaoService.ObterConfiguracaoPorEmpresaIdAsync(empresaId);
        }
        catch (Exception e)
        {
            log.LogError($"[ObterConfiguracaoPorEmpresaIdAsync-Erro] Erro: {e.Message}", e);
            notificador.Handle(new Notificacao("Ocorreu um problema",
                "Tivemos um problema para buscar a configuração",
                TipoNotificacao.Erro,
                HttpStatusCode.InternalServerError));
            return [];
        }
    }
    
    #endregion
}