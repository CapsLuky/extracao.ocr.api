namespace API.Contratual.Dto.Pesquisa;

public class PaginaDocumentoDto
{
    public Guid Id { get; set; }
    public Guid ArquivoId { get; set; }
    public short NumeroPagina { get; set; }
    public string NomeArquivo { get; set; }
    public string Palavra { get; set; }
    public string TrechoTexto { get; set; }
    public bool Ativo { get; set; }
    public double Relevancia { get; set; }
}
