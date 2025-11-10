namespace API.Contratual.Dto.Configuracao;

public class ConfiguracaoDto
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public Guid FilialId { get; set; }
    public string? Chave { get; set; }
    public string Valor { get; set; }
    public string? Descricao { get; set; }
    public ConfiguracaoEnum TipoValor { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime DataAtualizacao { get; set; }
}