using API.Contratual.Domain.Configuracao.Entity;
using API.Contratual.Domain.Configuracao.Payload;
using API.Contratual.Dto.Configuracao;

namespace API.Contratual.Domain.Configuracao;

public interface IConfiguracaoService
{
    Task<ConfiguracaoPastaDto?> InserirConfiguracaoPastaAsync(ConfiguracaoPastaIdsPayload configuracao);
    Task<ConfiguracaoPastaDto?> AtualizarConfiguracaoPastaAsync(Guid id, ConfiguracaoPastaPatchPayload configuracao);
    Task<ConfiguracaoPastaDto?> ObterConfiguracaoPastaPorIdAsync(Guid id);
    Task<ConfiguracaoPastaDto> ObterConfiguracaoPastaPorEmpresaIdFilialIdAsync(Guid empresaId, Guid filialId);
    Task<ConfiguracaoDto?> InserirConfiguracaoAsync(ConfiguracaoPaylod configuracao);
    Task<ConfiguracaoDto?> AtualizarConfiguracaoAsync(Guid id, ConfiguracaoPatchPayload configuracao);
    Task<ConfiguracaoDto?> ObterConfiguracaoPorIdAsync(Guid id);
    Task<List<ConfiguracaoDto>> ObterConfiguracaoPorEmpresaIdAsync(Guid empresaId);
}