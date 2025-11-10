using System.Net;
using API.Contratual.CrossCutting.Notificador;
using API.Contratual.Domain.Empresa;
using API.Contratual.Domain.Empresa.Payload;
using API.Contratual.Dto.Empresa;
using Microsoft.Extensions.Logging;

namespace API.Contratual.Application;

public class EmpresaApplication(ILogger<EmpresaApplication> log, IEmpresaService empresaService, INotificador notificador)
{
    public async Task<EmpresaDto?> InserirEmpresaAsync(EmpresaPayload empresa)
    {
        try
        {
            var empresaVm = await empresaService.InserirEmpresaAsync(empresa);

            if (empresaVm != null && !Guid.TryParse(empresaVm.Id.ToString(), out Guid _))
            {
                log.LogError($"[InserirEmpresaAsync-Erro] Erro ao inserir empresa : {empresaVm.Id}, {empresaVm.RazaoSocial}");
                
                if (notificador.ObterNotificacoes().Count != 0) return null;
                
                notificador.Handle(new Notificacao("Ocorreu um problema",
                    "Tivemos um problema para criar a empresa",
                    TipoNotificacao.Erro,
                    HttpStatusCode.InternalServerError));
                
                return null;
            }
            
            return empresaVm;
        }
        catch (Exception e)
        {
            log.LogError($"[InserirEmpresaAsync-Erro] Erro: {e.Message}", e);
            notificador.Handle(new Notificacao("Ocorreu um problema",
                "Tivemos um problema para criar a empresa",
                TipoNotificacao.Erro,
                HttpStatusCode.InternalServerError));
            return null;
        }
    }
    
    public async Task<EmpresaDto?> ObterEmpresaPorIdAsync(Guid id)
    {
        try
        {
            return await empresaService.ObterEmpresaPorIdAsync(id);
        }
        catch (Exception e)
        {
            log.LogError($"[ObterEmpresaPorIdAsync-Erro] Erro: {e.Message}", e);
            notificador.Handle(new Notificacao(HttpStatusCode.InternalServerError));
            return null;
        }
    }
    
    public async Task<EmpresaDto?> ObterEmpresaPorCnpjAsync(string cnpj)
    {
        try
        {
            return await empresaService.ObterEmpresaPorCnpjAsync(cnpj);
        }
        catch (Exception e)
        {
            log.LogError($"[ObterEmpresaPorCnpjAsync-Erro] Erro: {e.Message}", e);
            notificador.Handle(new Notificacao(HttpStatusCode.InternalServerError));
            return null;
        }
    }
    
    public async Task<EmpresaDto?> AtualizarEmpresaAsync(Guid id, EmpresaPayload empresa)
    {
        try
        {
            return await empresaService.AtualizarEmpresaAsync(id, empresa);
        }
        catch (Exception e)
        {
            log.LogError($"[EditarEmpresaAsync-Erro] Erro: {e.Message}", e);
            notificador.Handle(new Notificacao("Ocorreu um problema",
                "Tivemos um problema para atualizar as informações",
                TipoNotificacao.Erro,
                HttpStatusCode.InternalServerError));
            return null;
        }
    }
    
    public async Task<FilialDto?> InserirFilialAsync(FilialPayload filial)
    {
        try
        {
            var filialVm = await empresaService.InserirFilialAsync(filial);

            if (filialVm != null) return filialVm;
            log.LogError($"[InserirFilialAsync-Erro] Erro ao criar registro, nome: {filial.Nome}");
                
            if (notificador.ObterNotificacoes().Count != 0) return null;
                
            notificador.Handle(new Notificacao("Ocorreu um problema",
                "Tivemos um problema para criar o registro",
                TipoNotificacao.Erro,
                HttpStatusCode.InternalServerError));
                
            return null;
        }
        catch (Exception e)
        {
            log.LogError($"[InserirFilialAsync-Erro] Erro: {e.Message}", e);
            notificador.Handle(new Notificacao(HttpStatusCode.InternalServerError));
            return null;
        }
    }
    
    public async Task<FilialDto?> ObterFilialPorIdAsync(Guid id)
    {
        try
        {
            return await empresaService.ObterFilialPorIdAsync(id);
        }
        catch (Exception e)
        {
            log.LogError($"[ObterFilialPorIdAsync-Erro] Erro: {e.Message}", e);
            notificador.Handle(new Notificacao(HttpStatusCode.InternalServerError));
            return null;
        }
    }
    
    public async Task<List<FilialDto>> ObterFiliaisPorEmpresaIdAsync(Guid empresaId)
    {
        try
        {
            return await empresaService.ObterFiliaisPorEmpresaIdAsync(empresaId);
        }
        catch (Exception e)
        {
            log.LogError($"[ObterFiliaisPorEmpresaIdAsync-Erro] Erro: {e.Message}", e);
            notificador.Handle(new Notificacao("Ocorreu um problema",
                "Tivemos um problema para obter os registros",
                TipoNotificacao.Erro,
                HttpStatusCode.InternalServerError));
            return [];
        }
    }

    public async Task<FilialDto?> AtualizarFilialAsync(Guid id, FilialPayload filial)
    {
        try
        {
            return await empresaService.AtualizarFilialAsync(id, filial);
        }
        catch (Exception e)
        {
            log.LogError($"[AtualizarFilialAsync-Erro] Erro: {e.Message}", e);
            notificador.Handle(new Notificacao("Ocorreu um problema",
                "Tivemos um problema para atualizar as informações",
                TipoNotificacao.Erro,
                HttpStatusCode.InternalServerError));
            return null;
        }
    }
}