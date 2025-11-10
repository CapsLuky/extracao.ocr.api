using API.Contratual.Domain.Payload;
using API.Contratual.Dto.Configuracao;

namespace API.Contratual.Domain.Configuracao.Payload;

public class ConfiguracaoPaylod : EmpresaFilialIdsPayload
{
    public string Chave { get; set; }
    public string Valor { get; set; }
    public string Descricao { get; set; }
    public ConfiguracaoEnum TipoValor { get; set; }
    public string Ativo { get; set; }
}