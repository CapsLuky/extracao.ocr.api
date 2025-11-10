using API.Contratual.Dto.Pesquisa;

namespace API.Contratual.Domain.Pesquisa.Model;

public class PaginaDocumentoModel
{
    public Guid Id { get;  set; }
    public Guid ArquivoId { get;  set; }
    public short NumeroPagina { get;  set; }
    public string NomeArquivo { get;  set; }
    public string Palavra { get; set; }
    public string TrechoTexto { get; set; }
    public string Conteudo { get;  set; }
    public bool Ativo { get;  set; }
    public double Relevancia { get;  set; }
    
    public static implicit operator PaginaDocumentoDto(PaginaDocumentoModel? documentoViewModel)
        => documentoViewModel == null
            ? new PaginaDocumentoDto()
            : new PaginaDocumentoDto
            {
                Id = documentoViewModel.Id,
                ArquivoId = documentoViewModel.ArquivoId,
                NumeroPagina = documentoViewModel.NumeroPagina,
                NomeArquivo = documentoViewModel.NomeArquivo,
                Palavra = documentoViewModel.Palavra,
                TrechoTexto = documentoViewModel.TrechoTexto,
                Ativo = documentoViewModel.Ativo,
                Relevancia = documentoViewModel.Relevancia
            };
    
    public void ExtrairTrecho(string palavra)
    {
        const int palavrasAoRedor = 10;
        
        if (string.IsNullOrWhiteSpace(Conteudo) || string.IsNullOrWhiteSpace(palavra))
            throw new ArgumentException("Texto e palavra não podem ser nulos ou vazios.");

        var palavras = Conteudo.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        var indicesPalavra = new List<int>();

        for (int i = 0; i < palavras.Length; i++)
        {
            if (palavras[i].Equals(palavra, StringComparison.OrdinalIgnoreCase))
                indicesPalavra.Add(i);
        }

        if (indicesPalavra.Count == 0)
            TrechoTexto = string.Empty;

        var resultado = new List<string>();
        
        foreach (var index in indicesPalavra)
        {
            int startIndex = Math.Max(index - palavrasAoRedor, 0);
            int endIndex = Math.Min(index + palavrasAoRedor, palavras.Length - 1);

            var trecho = new List<string>();

            for (int i = startIndex; i <= endIndex; i++)
            {
                trecho.Add(palavras[i]);
            }

            resultado.Add(string.Join(" ", trecho));
        }

        TrechoTexto = string.Join("\n-----\n", resultado);
    }
}