using API.Contratual.CrossCutting.Common;
using API.Contratual.Domain.Empresa.Payload;
using API.Contratual.Domain.Entity;
using API.Contratual.Dto.Empresa;

namespace API.Contratual.Domain.Empresa.Entity;

public class FilialEntity : EntityBase
{
    public Guid EmpresaId { get; private set; }
    public string Nome { get; private set; }
    public string? Cnpj { get; private set; }
    public bool Ativo { get; private set; }

    public static implicit operator FilialDto?(FilialEntity? filialEntity)
        => filialEntity == null
            ? null
            : new FilialDto
            {
                Id = filialEntity.Id,
                EmpresaId = filialEntity.EmpresaId,
                Nome = filialEntity.Nome,
                Cnpj = filialEntity.Cnpj,
                Ativo = filialEntity.Ativo,
                DataCriacao = filialEntity.DataCadastro,
                DataAlteracao = filialEntity.DataAtualizacao
            };
    
    public static class FilialEntityFactory
    {
        public static FilialEntity NovaFilial(Guid empresaId, string? cnpj, string nome, bool ativo)
        {
            var empresa = new FilialEntity
            {
                Id = Guid.CreateVersion7(),
                EmpresaId = empresaId,
                Nome = nome,
                Cnpj = cnpj,
                Ativo = ativo
            };
            
            return empresa;
        }
    }
    
    public void SetarDataDeAtualizacao()
    {
        DataAtualizacao = DateTime.UtcNow.ToDateTimeBrazil();
    }

    public void AtualizarFilial(FilialPayload filial)
    {
        Nome = string.IsNullOrEmpty(filial.Nome) ? Nome : filial.Nome;
        Cnpj = string.IsNullOrEmpty(filial.Cnpj) ? Cnpj : filial.Cnpj;

        if (filial.Ativo != null)
        {
            Ativo = (bool)filial.Ativo;
        }
        
        SetarDataDeAtualizacao();
    }
}