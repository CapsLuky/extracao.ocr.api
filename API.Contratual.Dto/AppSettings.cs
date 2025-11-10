namespace API.Contratual.Dto;

public class AppSettings
{
    public string Secret { get; set; }
    public int ExpirationHoras { get; set; }
    public string Emissor { get; set; }
    public string ValidoEm { get; set; }
    public string PastaOrigemDocumento { get; set; }
    public string PastaDestinoDocumentos { get; set; }
}