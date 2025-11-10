namespace API.Contratual.Domain.Pesquisa.Payload;

public record BuscarResultadoPesquisaPayload
{
    public Guid? DicionarioId { get; set; }
    public Guid? ArquivoId { get; set; }
    public bool Ativo { get; set; }
    public int PaginacaoQtdDePaginas { get; set; }
    public short PaginacaoPaginaSelecionada { get; set; }
}