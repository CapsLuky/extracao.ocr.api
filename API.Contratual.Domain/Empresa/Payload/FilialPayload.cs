namespace API.Contratual.Domain.Empresa.Payload;

public class FilialPayload
{
    public Guid EmpresaId { get; set; }
    public string? Nome { get; set; }
    public string? Cnpj { get; set; }
    public bool? Ativo { get; set; }
}