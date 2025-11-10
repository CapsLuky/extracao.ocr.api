namespace API.Contratual.Domain.Empresa.Payload;

public class EmpresaPayload
{
    public string? RazaoSocial { get; set; }
    public string? Cnpj { get; set; }
    public bool? Ativo { get; set; }
}