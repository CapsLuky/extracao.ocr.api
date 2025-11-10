using API.Contratual.Domain.Payload;

namespace API.Contratual.Domain.Pesquisa.Payload;

public class ObterImagemPaginaPayload : EmpresaFilialIdsPayload
{
    public Guid ArquivoId { get; set; }
    public int NumeroPagina { get; set; }
}