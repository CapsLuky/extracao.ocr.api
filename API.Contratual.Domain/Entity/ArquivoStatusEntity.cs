using API.Contratual.Domain.Enum;

namespace API.Contratual.Domain.Entity;

public class ArquivoStatusEntity
{
    public EstadoArquivo Valor { get; private set; }
    public string Status { get; private set; }
}