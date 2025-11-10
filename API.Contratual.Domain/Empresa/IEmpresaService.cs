using API.Contratual.Domain.Empresa.Payload;
using API.Contratual.Domain.Entity;
using API.Contratual.Dto.Empresa;

namespace API.Contratual.Domain.Empresa;

public interface IEmpresaService
{
    Task<EmpresaDto?> InserirEmpresaAsync(EmpresaPayload empresa);
    Task<EmpresaDto?> AtualizarEmpresaAsync(Guid id, EmpresaPayload empresa);
    Task<EmpresaDto?> ObterEmpresaPorIdAsync(Guid id);
    Task<EmpresaDto?> ObterEmpresaPorCnpjAsync(string cnpj);
    Task<FilialDto?> InserirFilialAsync(FilialPayload filial);
    Task<FilialDto?> AtualizarFilialAsync(Guid id, FilialPayload filial);
    Task<FilialDto?> ObterFilialPorIdAsync(Guid id);
    Task<List<FilialDto>> ObterFiliaisPorEmpresaIdAsync(Guid empresaId);
}