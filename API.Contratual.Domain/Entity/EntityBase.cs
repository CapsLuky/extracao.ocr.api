namespace API.Contratual.Domain.Entity;

public class EntityBase
{
    public Guid Id { get; protected set; }
    public DateTime DataCadastro { get; protected set; }
    public DateTime DataAtualizacao { get; protected set; }
}