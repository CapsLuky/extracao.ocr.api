using API.Contratual.Domain.Payload;

namespace API.Contratual.Domain.Pesquisa.Payload;

public class EmpresaPalavra : EmpresaFilialIdsPayload
{
    public bool Ativo { get; set; }
}