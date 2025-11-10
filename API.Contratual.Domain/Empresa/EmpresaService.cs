using API.Contratual.Domain.Empresa.Entity;
using API.Contratual.Domain.Empresa.Payload;
using API.Contratual.Domain.Entity;
using API.Contratual.Domain.Interface.Repository;
using API.Contratual.Dto.Empresa;

namespace API.Contratual.Domain.Empresa;

public class EmpresaService(IEmpresaRepository empresaRepository) : IEmpresaService
{ 
    public async Task<EmpresaDto?> InserirEmpresaAsync(EmpresaPayload empresa)
    {
        var empresaEntity = EmpresaEntity.
            EmpresaEntityFactory.
            NovaEmpresa(empresa.RazaoSocial, empresa.Cnpj, Convert.ToBoolean(empresa.Ativo));
        
        var response = await empresaRepository.InserirEmpresaAsync(empresaEntity);
        
        if(response)
            return await empresaRepository.ObterEmpresaPorIdAsync(empresaEntity.Id);
        
        return null;
    }

    public async Task<EmpresaDto?> AtualizarEmpresaAsync(Guid id, EmpresaPayload empresaPayload)
    {
        var empresaEntity = await empresaRepository.ObterEmpresaPorIdAsync(id);
        
        if (empresaEntity == null) return null;
        
        empresaEntity.AtualizarEmpresa(empresaPayload);
        
        var response = await empresaRepository.AtualizarEmpresaAsync(empresaEntity);
        return response;
    }

    public async Task<EmpresaDto?> ObterEmpresaPorIdAsync(Guid id)
    {
        return await empresaRepository.ObterEmpresaPorIdAsync(id);
    }

    public async Task<EmpresaDto?> ObterEmpresaPorCnpjAsync(string cnpj)
    {
        var response = await empresaRepository.ObterEmpresaPorCnpjAsync(cnpj);
        return response;
    }

    public async Task<FilialDto?> InserirFilialAsync(FilialPayload filial)
    {
        var filialEntity = FilialEntity.FilialEntityFactory.NovaFilial(
            filial.EmpresaId,
            filial.Cnpj,
            filial.Nome,
            Convert.ToBoolean(filial.Ativo));
        
        var response = await empresaRepository.InserirFilialAsync(filialEntity);

        if (response)
            return await empresaRepository.ObterFilialPorIdAsync(filialEntity.Id);
        
        return null;
    }

    public async Task<FilialDto?> AtualizarFilialAsync(Guid id, FilialPayload filial)
    {
        var filialEnetity = await empresaRepository.ObterFilialPorIdAsync(id);
        
        if (filialEnetity == null) return null;
        
        filialEnetity.AtualizarFilial(filial);
        
        return await empresaRepository.AtualizarFilialAsync(filialEnetity);
    }

    public async Task<FilialDto?> ObterFilialPorIdAsync(Guid id)
    {
        return await empresaRepository.ObterFilialPorIdAsync(id);
    }
    
    public async Task<List<FilialDto>> ObterFiliaisPorEmpresaIdAsync(Guid empresaId)
    {
        var filialEntity = await empresaRepository.ObterFiliaisPorEmpresaIdAsync(empresaId);
        return filialEntity.Select(f => (FilialDto)f).ToList();
    }
}