using API.Contratual.CrossCutting.Common;
using API.Contratual.Domain.Configuracao.Payload;
using API.Contratual.Domain.Entity;
using API.Contratual.Dto.Configuracao;

namespace API.Contratual.Domain.Configuracao.Entity;

public class ConfiguracaoPastaEntity : EntityBase
{
    public Guid EmpresaId { get; private set; }
    public Guid FilialId { get; private set; }
    public string PastaOrigem { get; private set; }
    public string PastaDestino { get; private set; }
    public bool CriarCopia { get; private set; }
    public bool Ativo { get; private set; } = true;
    
    public void SetarDataDeAtualizacao()
    {
        DataAtualizacao = DateTime.UtcNow.ToDateTimeBrazil();
    }
    
    public void AtualizarConfiguracaoPasta(ConfiguracaoPastaPatchPayload configuracao)
    {
        PastaOrigem = string.IsNullOrEmpty(configuracao.PastaOrigem) ? PastaOrigem : configuracao.PastaOrigem;
        PastaDestino = string.IsNullOrEmpty(configuracao.PastaDestino) ? PastaDestino : configuracao.PastaDestino;
    
        if (configuracao.CriarCopia != null)
        {
            CriarCopia = (bool)configuracao.CriarCopia;
        }
        
        if (configuracao.Ativo != null)
        {
            Ativo = (bool)configuracao.Ativo;
        }

        if (string.IsNullOrEmpty(configuracao.PastaDestino))
        {
            CriarCopia = false;
        }
        
        SetarDataDeAtualizacao();
    }
    
    public static implicit operator ConfiguracaoPastaDto?(ConfiguracaoPastaEntity? configuracao)
        => configuracao == null 
            ? null 
            : new ConfiguracaoPastaDto
                {
                    Id = configuracao.Id,
                    EmpresaId = configuracao.EmpresaId,
                    FilialId = configuracao.FilialId,
                    PastaOrigem = configuracao.PastaOrigem,
                    PastaDestino = configuracao.PastaDestino,
                    CriarCopia = configuracao.CriarCopia,
                    Ativo = configuracao.Ativo,
                    DataAtualizacao = configuracao.DataAtualizacao,
                    DataCadastro = configuracao.DataCadastro
                };
    
    public static class Factory
    {
        public static ConfiguracaoPastaEntity NovaConfiguracao(
            Guid empresaId, 
            Guid filialId,
            string pastaOrigem,
            string pastaDestino,
            bool criarCopia,
            bool ativo)
        {
            if (string.IsNullOrEmpty(pastaDestino))
            {
                criarCopia = false;
            }
            
            var configuracao = new ConfiguracaoPastaEntity
            {
                Id = Guid.CreateVersion7(),
                EmpresaId = empresaId,
                FilialId = filialId,
                PastaOrigem = pastaOrigem,
                PastaDestino = pastaDestino,
                CriarCopia = criarCopia,
                Ativo = ativo
            };
            
            return configuracao;
        }
    }
}