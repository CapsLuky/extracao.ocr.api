namespace API.Contratual.Domain.Entity;

public class DicionarioEntity : EntityBase
{
    public Guid EmpresaId { get; private set; }
    public Guid FilialId { get; private set; }
    public string Palavra { get; private set; }
    public bool Ativo { get; private set; }
    
    public static class DicionarioEntiryFactory
    {
        public static DicionarioEntity Novo(string palavra, bool ativo, Guid empresaId, Guid filialId)
        {
            var arquivo = new DicionarioEntity
            {
                Id = Guid.CreateVersion7(),
                EmpresaId = empresaId,
                FilialId = filialId,
                Palavra = palavra,
                Ativo = ativo
            };

            return arquivo;
        }
    }
}