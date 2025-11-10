namespace API.Contratual.Domain.Configuracao.Payload;

public class ConfiguracaoPastaPatchPayload
{
    public string? PastaOrigem { get; set; }
    public string? PastaDestino { get; set; }
    public bool? CriarCopia { get; set; }
    public bool? Ativo { get; set; }
}