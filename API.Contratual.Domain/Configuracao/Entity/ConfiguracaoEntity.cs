using API.Contratual.CrossCutting.Common;
using API.Contratual.Domain.Configuracao.Payload;
using API.Contratual.Domain.Entity;
using API.Contratual.Dto.Configuracao;

namespace API.Contratual.Domain.Configuracao.Entity;

public class ConfiguracaoEntity : EntityBase
{
    public Guid EmpresaId { get; private set; }
    public Guid FilialId { get; private set; }
    public string? Chave { get; private set; }
    public string? Valor { get; private set; }
    public string? Descricao { get; private set; }
    public ConfiguracaoEnum TipoValor { get; private set; }
    public bool Ativo { get; private set; }
    
    public static class Factory
    {
        public static ConfiguracaoEntity NovaConfiguracao(
            Guid empresaId, 
            Guid filialId,
            string chave,
            string valor,
            string descricao,
            ConfiguracaoEnum tipoValor,
            bool ativo)
        {
            var configuracao = new ConfiguracaoEntity
            {
                Id = Guid.CreateVersion7(),
                EmpresaId = empresaId,
                FilialId = filialId,
                Chave = chave,
                Valor = valor,
                Descricao = descricao,
                TipoValor = tipoValor,
                Ativo = ativo
            };
            
            return configuracao;
        }
    }

    public static implicit operator ConfiguracaoDto?(ConfiguracaoEntity? configuracao)
        => configuracao == null 
            ? null 
            : new ConfiguracaoDto
                {
                    Id = configuracao.Id,
                    EmpresaId = configuracao.EmpresaId,
                    FilialId = configuracao.FilialId,
                    Chave = configuracao.Chave,
                    Valor = configuracao.Valor,
                    Descricao = configuracao.Descricao,
                    TipoValor = configuracao.TipoValor,
                    Ativo = configuracao.Ativo,
                    DataAtualizacao = configuracao.DataAtualizacao,
                    DataCadastro = configuracao.DataCadastro
                };
    public void SetarDataDeAtualizacao()
    {
        DataAtualizacao = DateTime.UtcNow.ToDateTimeBrazil();
    }

    public void AtualizarConfiguracao(ConfiguracaoPatchPayload configuracao)
    {
        var tipoValor = TipoValor;
        if (configuracao.TipoValor != null) tipoValor = (ConfiguracaoEnum)configuracao.TipoValor;
        
        Valor = string.IsNullOrEmpty(configuracao.Valor) ? Valor : configuracao.Valor;
        Descricao = string.IsNullOrEmpty(configuracao.Descricao) ? Descricao : configuracao.Descricao;
        TipoValor = tipoValor;
        Ativo = string.IsNullOrEmpty(configuracao.Ativo) ? Convert.ToBoolean(Ativo) : Convert.ToBoolean(configuracao.Ativo);
        SetarDataDeAtualizacao();
    }
}