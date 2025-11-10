using API.Contratual.Dto.Configuracao;

namespace API.Contratual.Domain.Configuracao.Payload;

public class ConfiguracaoPatchPayload
{
    public string? Valor { get; set; }
    public string? Descricao { get; set; }
    public ConfiguracaoEnum? TipoValor { get; set; }
    public string? Ativo { get; set; }
}