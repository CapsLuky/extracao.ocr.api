namespace API.Contratual.Dto.Empresa;

public class EmpresaDto
{
    public Guid Id { get; set; }
    public string RazaoSocial { get; set; }
    public string Cnpj { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime DataAtualizado { get; set; }
}