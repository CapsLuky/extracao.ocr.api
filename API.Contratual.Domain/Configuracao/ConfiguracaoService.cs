using API.Contratual.Domain.Configuracao.Entity;
using API.Contratual.Domain.Configuracao.Payload;
using API.Contratual.Domain.Entity;
using API.Contratual.Domain.Interface.Repository;
using API.Contratual.Dto.Configuracao;

namespace API.Contratual.Domain.Configuracao;

public class ConfiguracaoService(IConfiguracaoReposity configuracaoReposity) : IConfiguracaoService
{
    #region Configuração pasta

    public async Task<ConfiguracaoPastaDto?> InserirConfiguracaoPastaAsync(ConfiguracaoPastaIdsPayload configuracao)
    {
        var configuracaoPasta = ConfiguracaoPastaEntity.Factory.NovaConfiguracao(
            empresaId: configuracao.EmpresaId,
            filialId: configuracao.FilialId,
            pastaOrigem: configuracao.PastaOrigem,
            pastaDestino: configuracao.PastaDestino,
            criarCopia: configuracao.CriarCopia,
            ativo: configuracao.Ativo
        );

        var res = await configuracaoReposity.InserirConfiguracaoPastaAsync(configuracaoPasta);

        return res ? configuracaoPasta : null;
    }
    
    public async Task<ConfiguracaoPastaDto?> AtualizarConfiguracaoPastaAsync(Guid id, ConfiguracaoPastaPatchPayload configuracao)
    {
        var configuracaoPasta = await configuracaoReposity.ObterConfiguracaoPastaPorIdAsync(id);

        if (configuracaoPasta == null) return null;

        configuracaoPasta.AtualizarConfiguracaoPasta(configuracao);

        var res = await configuracaoReposity.AtualizarConfiguracaoPastaAsync(configuracaoPasta);

        if (res)
        {
            return await configuracaoReposity.ObterConfiguracaoPastaPorIdAsync(id);
        }

        return null;
    }
    
    public async Task<ConfiguracaoPastaDto?> ObterConfiguracaoPastaPorIdAsync(Guid id)
    {
        return await configuracaoReposity.ObterConfiguracaoPastaPorIdAsync(id);
    }
    
    public async Task<ConfiguracaoPastaDto> ObterConfiguracaoPastaPorEmpresaIdFilialIdAsync(Guid empresaId, Guid filialId)
    {
        return await configuracaoReposity.ObterConfiguracaoPastaPorEmpresaIdFilialIdAsync(empresaId, filialId);
    }

    #endregion

    #region Configuração

    public async Task<ConfiguracaoDto?> InserirConfiguracaoAsync(ConfiguracaoPaylod configuracao)
    {
        var configuracaoEntity = ConfiguracaoEntity.Factory.NovaConfiguracao(
            empresaId: configuracao.EmpresaId,
            filialId: configuracao.FilialId,
            chave: configuracao.Chave,
            valor: configuracao.Valor,
            descricao: configuracao.Descricao,
            tipoValor: configuracao.TipoValor,
            ativo: Convert.ToBoolean(configuracao.Ativo)
        );

        var res = await configuracaoReposity.InserirConfiguracaoAsync(configuracaoEntity);

        return res ? configuracaoEntity : null;
    }

    public async Task<ConfiguracaoDto?> AtualizarConfiguracaoAsync(Guid id, ConfiguracaoPatchPayload configuracao)
    {
        var configuracaoEntity = await configuracaoReposity.ObterConfiguracaoPorIdAsync(id);

        if (configuracaoEntity == null) return null;

        configuracaoEntity.AtualizarConfiguracao(configuracao);

        var res = await configuracaoReposity.AtualizarConfiguracaoAsync(configuracaoEntity);

        if (res)
        {
            return await configuracaoReposity.ObterConfiguracaoPorIdAsync(id);
        }

        return null;
    }

    public async Task<ConfiguracaoDto?> ObterConfiguracaoPorIdAsync(Guid id)
    {
        return await configuracaoReposity.ObterConfiguracaoPorIdAsync(id);
    }

    public async Task<List<ConfiguracaoDto>> ObterConfiguracaoPorEmpresaIdAsync(Guid empresaId)
    {
        var res = await configuracaoReposity.ObterConfiguracaoPorEmpresaIdAsync(empresaId);
        return res.Select(f => (ConfiguracaoDto)f).ToList();
    }

    #endregion
}