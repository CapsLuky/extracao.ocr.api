using API.Contratual.CrossCutting.Common;
using API.Contratual.Domain.Empresa.Payload;
using API.Contratual.Domain.Entity;
using API.Contratual.Dto.Empresa;

namespace API.Contratual.Domain.Empresa.Entity;

public class EmpresaEntity : EntityBase
{
    public string RazaoSocial { get; private set; }
    public string Cnpj { get; private set; }
    public bool Ativo { get; private set; }

    public static implicit operator EmpresaDto?(EmpresaEntity? empresaEntity)
        => empresaEntity == null
            ? null
            : new EmpresaDto
            {
                Id = empresaEntity.Id,
                RazaoSocial = empresaEntity.RazaoSocial,
                Cnpj = empresaEntity.Cnpj,
                Ativo = empresaEntity.Ativo,
                DataAtualizado = empresaEntity.DataAtualizacao,
                DataCadastro = empresaEntity.DataCadastro
            };
    
    public static class EmpresaEntityFactory
    {
        public static EmpresaEntity NovaEmpresa(string razaoSocial, string cnpj, bool ativo)
        {
            var empresa = new EmpresaEntity
            {
                Id = Guid.CreateVersion7(),
                RazaoSocial = razaoSocial,
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

    public void AtualizarEmpresa(EmpresaPayload empresa)
    {
        RazaoSocial = string.IsNullOrEmpty(empresa.RazaoSocial) ? RazaoSocial : empresa.RazaoSocial;
        Cnpj = string.IsNullOrEmpty(empresa.Cnpj) ? Cnpj : empresa.Cnpj;

        if (empresa.Ativo != null)
        {
            Ativo = (bool)empresa.Ativo;
        }
        
        SetarDataDeAtualizacao();
    }
}