namespace API.Contratual.Dto.Configuracao;

public class ConfiguracaoPastaDto
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public Guid FilialId { get; set; }
    public string PastaOrigem { get; set; }
    public string PastaDestino { get; set; }
    public bool CriarCopia { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime DataAtualizacao { get; set; }
}