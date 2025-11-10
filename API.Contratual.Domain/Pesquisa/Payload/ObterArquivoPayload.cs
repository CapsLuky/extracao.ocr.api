using API.Contratual.Domain.Payload;

namespace API.Contratual.Domain.Pesquisa.Payload;

public class ObterArquivoPayload : EmpresaFilialIdsPayload
{
    public Guid ArquivoId { get; set; }
}