using API.Contratual.CrossCutting.Common;
using API.Contratual.Domain.Enum;

namespace API.Contratual.Domain.Entity;

public class ArquivoEntity
{
    public Guid Id { get; private set; }
    public Guid EmpresaId { get; set; }
    public Guid FilialId { get; set; }
    public Guid? PastaVirtualId { get; set; }
    public EstadoArquivo EstadoArquivoId { get; set; }
    public string NomeOriginal { get; set; }
    public string? NomeCopia { get; set; } = string.Empty;
    public string CaminhoFisico { get; set; }
    public string TipoArquivo { get; set; }
    public long TamanhoBytes { get; set; }
    public DateTime DataUpload { get; set; }
    public DateTime DataAtualizacao { get; set; }
    public DateTime DataProcessamento { get; private set; }

    protected ArquivoEntity()
    {
    }

    private readonly List<ArquivoPaginaEntity> _arquivoPaginas;
    public IReadOnlyCollection<ArquivoPaginaEntity> ArquivoPaginas => _arquivoPaginas;

    public void SetarNomeOriginalDoArquivo(string nomeArquivo)
    {
        NomeOriginal = nomeArquivo;
    }
    
    public void SetarLocalDoArquivo(string caminhoFisico)
    {
        CaminhoFisico = caminhoFisico;
    }

    public void SetarTipoDoArquivo(string tipoArquivo)
    {
        TipoArquivo = tipoArquivo;
    }

    public void SetarEstadoArquivo(EstadoArquivo status)
    {
        EstadoArquivoId = status;
    }

    public void SetarDataProcessamento()
    {
        DataProcessamento = DateTime.UtcNow.ToDateTimeBrazil();
    }

    public void SetarDataUpload()
    {
        DataUpload = DateTime.UtcNow.ToDateTimeBrazil();
    }

    public static class ArquivoFactory
    {
        public static ArquivoEntity NovoArquivo(Guid empresaId, Guid filialId, string nomeArquivo, long tamanhoBytes, bool criarCopia)
        {
            var novoId = Guid.CreateVersion7();
            var arquivo = new ArquivoEntity
            {
                Id = novoId,
                EmpresaId = empresaId,
                FilialId = filialId,
                NomeOriginal = nomeArquivo, 
                NomeCopia = criarCopia ? novoId + "_" + nomeArquivo : string.Empty,
                TamanhoBytes = tamanhoBytes,
                DataProcessamento = DateTime.UtcNow.ToDateTimeBrazil(),
                EstadoArquivoId = EstadoArquivo.ProntoParaEstrairTexto
            };

            return arquivo;
        }
    }
}