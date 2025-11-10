using System.Net;
using API.Contratual.Domain.Entity;
using API.Contratual.Domain.Payload;
using API.Contratual.Domain.Pesquisa.Model;
using API.Contratual.Domain.Pesquisa.Payload;
using API.Contratual.Dto.Pesquisa;

namespace API.Contratual.Domain.Pesquisa;

public interface IPesquisaService
{
    Task<IEnumerable<ArquivoEntity>> ObterTodosDocumentosAsync(EmpresaFilialIdsPayload ids);
    Task<IEnumerable<PaginaDocumentoModel>> BuscarResultadoPesquisaAsync(BuscarResultadoPesquisaPayload payload);
    Task<IEnumerable<PaginaDocumentoDto>> PesquisarAsync(string textos, Guid arquivoId);
    Task<(byte[], HttpStatusCode, string)> BuscarArquivoPdfPorIdAsync(Guid arquivoId, bool copia);
    Task<IEnumerable<DicionarioEntity>> ObterPalavrasAsync(EmpresaPalavra empresaPalavra);
    Task<int> AtualizarValidadeResultadoPesquisaAsync(Guid id, bool ativo);
    byte[] ObterImagemDoPdfAsync(byte[] arquivoPdf, int numeroPagina);
}