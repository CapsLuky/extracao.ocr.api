using API.Contratual.Domain.Configuracao.Entity;
using API.Contratual.Domain.Entity;

namespace API.Contratual.Domain.Interface.Repository;

public interface IConfiguracaoReposity
{
    #region Configuração pasta
    
    Task<bool> InserirConfiguracaoPastaAsync(ConfiguracaoPastaEntity configuracaoPasta);
    Task<bool> AtualizarConfiguracaoPastaAsync(ConfiguracaoPastaEntity configuracaoPasta);
    Task<ConfiguracaoPastaEntity> ObterConfiguracaoPastaPorEmpresaIdFilialIdAsync(Guid empresaId, Guid filialId);
    Task<ConfiguracaoPastaEntity?> ObterConfiguracaoPastaPorIdAsync(Guid id);
    
    #endregion

    #region Configuração

    Task<bool> InserirConfiguracaoAsync(ConfiguracaoEntity configuracao);
    Task<bool> AtualizarConfiguracaoAsync(ConfiguracaoEntity configuracao);
    
    Task<ConfiguracaoEntity?> ObterConfiguracaoPorIdAsync(Guid id);

    Task<IEnumerable<ConfiguracaoEntity>> ObterConfiguracaoPorEmpresaIdAsync(Guid empresaId);
    
    #endregion
}