using API.Contratual.Dto.Pesquisa;

namespace API.Contratual.Domain.Entity;

public class ResultadoPesquisaEntity
{
    public Guid Id { get; private set; }
    public string TrechoTexto { get; private set; }
    public Guid DicionarioPalavraId { get; private set; }
    public Guid ArquivoId { get;  private set; }
    public Guid ArquivoPaginaId { get; private set; }
    public bool Ativo { get; private set; }
    public double Relevancia { get;  private set; }
    
    protected ResultadoPesquisaEntity(){}

    public void SetarDicionarioId(Guid dicionarioId)
    {
        DicionarioPalavraId = dicionarioId;
    }

    public static class ResultadoPesquisaFactory
    {
        public static ResultadoPesquisaEntity NovoResultadoPesquisa(PaginaDocumentoDto dto)
        {
            var resultado = new ResultadoPesquisaEntity
            {
                Id = Guid.CreateVersion7(),
                TrechoTexto = dto.TrechoTexto,
                ArquivoId = dto.ArquivoId,
                ArquivoPaginaId = dto.Id,
                Relevancia = dto.Relevancia,
                Ativo = true
            };

            return resultado;
        }
    }
}