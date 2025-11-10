using API.Contratual.Domain.Empresa.Entity;
using API.Contratual.Domain.Entity;
using API.Contratual.Dto.Empresa;

namespace API.Contratual.Domain.Interface.Repository;

public interface IEmpresaRepository
{
    Task<bool> InserirEmpresaAsync(EmpresaEntity empresa);
    Task<EmpresaEntity?> AtualizarEmpresaAsync(EmpresaEntity empresa);
    Task<EmpresaEntity?> ObterEmpresaPorIdAsync(Guid id);
    Task<EmpresaEntity?> ObterEmpresaPorCnpjAsync(string cnpj);
    Task<bool> InserirFilialAsync(FilialEntity filialEntity);
    Task<FilialEntity?> AtualizarFilialAsync(FilialEntity filial);
    Task<FilialEntity?> ObterFilialPorIdAsync(Guid id);
    Task<IEnumerable<FilialEntity>> ObterFiliaisPorEmpresaIdAsync(Guid empresaId);
}