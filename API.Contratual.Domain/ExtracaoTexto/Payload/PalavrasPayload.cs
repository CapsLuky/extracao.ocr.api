namespace API.Contratual.Domain.ExtracaoTexto.Payload;

public class PalavrasPayload
{
    public Guid EmpresaId { get; set; }
    public Guid FilialId { get; set; }
    public List<string> Palavras { get; set; }
}