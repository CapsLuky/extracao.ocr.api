namespace API.Contratual.CrossCutting.Common.Model;

public class ArquivoValidacaoInfo
{
    public string CaminhoCompleto { get; set; }
    public string NomeArquivo { get; set; }
    public int TamanhoNome { get; set; }
    public int LimiteNome { get; set; }
    public string CaminhoFisico { get; set; }
    public int TamanhoCaminho { get; set; }
    public int LimiteCaminho { get; set; }
    public long TamanhoArquivoBytes { get; set; }
}