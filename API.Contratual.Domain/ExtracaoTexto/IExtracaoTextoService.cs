using API.Contratual.Domain.Entity;
using API.Contratual.Domain.Enum;
using API.Contratual.Domain.ExtracaoTexto.Payload;
using API.Contratual.Dto.Configuracao;

namespace API.Contratual.Domain.ExtracaoTexto;

public interface IExtracaoTextoService
{
    List<string> BuscarArquivosRecursivamente(string pasta);
    Task<string> ExtrairTextoDasPaginasDosArquivosPdfAsync(List<ArquivoEntity> arquivoList);
    Task<List<ArquivoEntity>> SelecionarArquivosParaExtrairTextoAsync(ConfiguracaoPastaDto configPasta, List<string> arquivosNaPasta);
    Task ApagarPaginasPorArquivoIdAsync(List<ArquivoEntity> arquivoList);
    Task<int> InserirArquivoAsync(List<ArquivoEntity> arquivoList);
    Task<List<ArquivoEntity>> ObterArquivosParaProcessamentoAsync(EstadoArquivo status);
    Task<int> InserirResultadoPesquisaAsync(List<ResultadoPesquisaEntity> resultadoPesquisaEntities);
    Task FazerCopiaDosArquivos(List<ArquivoEntity> arquivoList, ConfiguracaoPastaDto configPasta);

    Task<ResultadoPesquisaEntity?> BuscarResultadoPesquisaPorDicionaroIdArquivoIdAsync(Guid dicionarioId,
        Guid arquivoId);

    Task<int> InserirPalavrasParaPrePesquisa(PalavrasPayload palavrasPayload);
}