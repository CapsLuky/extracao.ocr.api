using API.Contratual.Domain.Entity;
using API.Contratual.Domain.Enum;
using API.Contratual.Domain.Payload;
using API.Contratual.Domain.Pesquisa.Model;
using API.Contratual.Domain.Pesquisa.Payload;

namespace API.Contratual.Domain.Interface.Repository;

public interface IDocumentoRepository
{
    Task<IEnumerable<PaginaDocumentoModel>> PesquisarPalavrasAsync(string palavras, Guid arquivoId, ModoDeBusca modoDeBusca);
    Task<int> InserirPaginaDocumento(List<ArquivoPaginaEntity> documentoList);

    Task<ArquivoPaginaEntity?> BuscarDocumentoPorIdAsync(int id);
    Task<ArquivoEntity?> BuscarArquivosProcessadosPorLocalEoNome(string caminhoFisico, string nomeOriginal);
    Task<bool> ApagarArquivoPaginaPorArquivoIdAsync(Guid arquivoId);
    Task<int> InserirArquivoAsync(List<ArquivoEntity> arquivosNovos);
    Task<IEnumerable<ArquivoEntity>?> ObterArquivoPorStatusAsync(EstadoArquivo status);
    Task<int> AtualizarArquivosAsync(List<ArquivoEntity> arquivoList);
    Task<IEnumerable<ArquivoEntity>> ObterTodosDocumentosAsync(EmpresaFilialIdsPayload idsPayload);
    Task<IEnumerable<DicionarioEntity>> ObterPalavrasAsync(EmpresaPalavra empresaPalavra);
    Task<ArquivoEntity?> ObterArquivoPorIdAsync(Guid id);
    Task<int> AtualizarValidadeResultadoPesquisaAsync(Guid id, bool ativo);
    Task<int> AtualizarArquivoAsync(ArquivoEntity arquivo);
    //Task<bool> ConsultarSeArquivoConstaNaTabela(string nomeOriginal, long tamanhoBytes);
    Task<ArquivoEntity?> BuscarArquivoProcessadoPorIdentificacao(string nomeOriginal, long tamanhoBytes);

    #region Tabela ResultadoPesquisa

    Task<int> InserirResultadoPesquisaAsync(List<ResultadoPesquisaEntity> resultadoPesquisaEntities);
    Task<IEnumerable<PaginaDocumentoModel>> BuscarResultadoPesquisaListAsync(BuscarResultadoPesquisaPayload payload);
    Task<ResultadoPesquisaEntity?> BuscarResultadoPesquisaPorDicionaroIdArquivoIdAsync(Guid dicionarioPalavraId, Guid arquivoId);

    #endregion
    
    #region tbl Dicionario

    Task<int> InserirPalavraDicionarioAsync(List<DicionarioEntity> dicionario);
    Task<bool> InativarPalavraDicionarioAsync();
    Task<bool> AtualizarPalavraDicionarioAsync();

    #endregion

    
}