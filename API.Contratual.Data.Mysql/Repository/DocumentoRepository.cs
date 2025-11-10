using System.Data;
using System.Text;
using API.Contratual.Data.Mysql.Connections;
using API.Contratual.Domain.Entity;
using API.Contratual.Domain.Enum;
using API.Contratual.Domain.Interface.Repository;
using API.Contratual.Domain.Payload;
using API.Contratual.Domain.Pesquisa.Model;
using API.Contratual.Domain.Pesquisa.Payload;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;
using Exception = System.Exception;

namespace API.Contratual.Data.Mysql.Repository;

public class DocumentoRepository(IOptions<ConnectionStrings> appSettings, ILogger<DocumentoRepository> log) : IDocumentoRepository
{
    public async Task<IEnumerable<PaginaDocumentoModel>> PesquisarPalavrasAsync(string palavras, Guid arquivoId, ModoDeBusca modoDeBusca)
    {
        var metodoDaPesquisa = modoDeBusca switch
        {
            ModoDeBusca.BooleanMode => "IN BOOLEAN MODE",
            ModoDeBusca.NaturalLanguageMode => "IN NATURAL LANGUAGE MODE",
            ModoDeBusca.QueryExpansion => "WITH QUERY EXPANSION",
            _ => "IN NATURAL LANGUAGE MODE"
        };

        var sql =
            @$"SELECT AP.id, 
                      A.id AS ArquivoId, 
                      A.nome_original, 
                      '{palavras}' AS Palavra, 
                      numero_pagina AS NumeroPagina, 
                      conteudo_texto AS Conteudo, 
                      MATCH(conteudo_texto) AGAINST ('{palavras}' {metodoDaPesquisa}) AS Relevancia
               FROM arquivo_pagina AS AP
               INNER JOIN arquivo AS A ON A.id = AP.arquivo_id
               WHERE MATCH(conteudo_texto) AGAINST ('{palavras}' {metodoDaPesquisa}) AND A.id = @arquivoId
               ORDER BY relevancia DESC;";

        await using var connection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();
        var result = await connection.QueryAsync<PaginaDocumentoModel>(sql: sql, param: new { arquivoId = arquivoId }, transaction, commandType: CommandType.Text);
        await transaction.CommitAsync();

        return !result.Any() ? Enumerable.Empty<PaginaDocumentoModel>() : result;
    }

    #region Tabela ResultadoPesquisa

    public async Task<int> InserirResultadoPesquisaAsync(List<ResultadoPesquisaEntity> resultadoPesquisaEntities)
    {
        const string sql = @"INSERT INTO resultado_pesquisa (id, trecho_texto, dicionario_palavra_id, arquivo_id, arquivo_pagina_id, ativo, Relevancia)
                             VALUES (@Id, @TrechoTexto, @DicionarioPalavraId, @ArquivoId, @ArquivoPaginaId, @Ativo, @Relevancia)";

        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        dbConnection.Open();

        using var transaction = dbConnection.BeginTransaction();
        try
        {
            int linhasInseridas = await dbConnection.ExecuteAsync(sql, resultadoPesquisaEntities, transaction: transaction);
            transaction.Commit();
            return linhasInseridas;
        }
        catch (Exception e)
        {
            log.LogError($"[InserirResultadoPesquisaAsync] Erro: {e.Message}", e);
            transaction.Rollback();
            return 0;
        }
    }

    public async Task<IEnumerable<PaginaDocumentoModel>> BuscarResultadoPesquisaListAsync(BuscarResultadoPesquisaPayload payload)
    {
        var sqlBuilder = new StringBuilder();

        sqlBuilder.Append(
            @"SELECT RP.id AS Id, 
                     RP.arquivo_id AS ArquivoId, 
                     A.nome_original AS NomeArquivo, 
                     D.palavra AS Palavra, 
                     AP.numero_pagina AS NumeroPagina, 
                     conteudo_texto AS Conteudo, 
                     RP.ativo AS Ativo, 
                     RP.Relevancia
            FROM resultado_pesquisa AS RP
            INNER JOIN arquivo AS A ON A.Id = RP.arquivo_id
            INNER JOIN arquivo_pagina AS AP ON AP.Id = RP.arquivo_pagina_id
            INNER JOIN dicionario_palavra as D ON D.Id = RP.dicionario_palavra_id
            WHERE RP.ativo = @Ativo");

        var parametros = new DynamicParameters();
        parametros.Add("Ativo", payload.Ativo);

        if (payload.DicionarioId.HasValue)
        {
            sqlBuilder.Append(" AND RP.dicionario_palavra_id = @DicionarioId");
            parametros.Add("DicionarioId", payload.DicionarioId);
        }

        if (payload.ArquivoId.HasValue)
        {
            sqlBuilder.Append(" AND RP.arquivo_id = @ArquivoId");
            parametros.Add("ArquivoId", payload.ArquivoId);
        }


        sqlBuilder.Append(" ORDER BY A.nome_original, D.palavra");
        sqlBuilder.Append(" LIMIT @PageSize OFFSET @Offset");

        parametros.Add("PageSize", payload.PaginacaoQtdDePaginas);
        parametros.Add("Offset", (payload.PaginacaoPaginaSelecionada - 1) * payload.PaginacaoQtdDePaginas);

        var sql = sqlBuilder.ToString();

        await using var connection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        await connection.OpenAsync();
        var result = await connection.QueryAsync<PaginaDocumentoModel>(sql, parametros);

        return result.Any() ? result : Enumerable.Empty<PaginaDocumentoModel>();
    }

    public async Task<ResultadoPesquisaEntity?> BuscarResultadoPesquisaPorDicionaroIdArquivoIdAsync(Guid dicionarioPalavraId, Guid arquivoId)
    {
        const string sql =
            @"SELECT id AS Id,
                 trecho_texto AS TrechoTexto, 
                 dicionario_palavra_id AS DicionarioPalavraId, 
                 arquivo_id AS ArquivoId, 
                 arquivo_pagina_id AS ArquivoPaginaId, 
                 ativo AS Ativo, 
                 relevancia AS Relevancia 
        FROM resultado_pesquisa 
        WHERE dicionario_palavra_id = @dicionarioPalavraId 
        AND arquivo_id = @arquivoId";

        await using var connection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        await connection.OpenAsync();
        
        var result = await connection.QueryFirstOrDefaultAsync<ResultadoPesquisaEntity>(
            sql, 
            new { dicionarioPalavraId, arquivoId });

        return result;
    }

    public async Task<int> AtualizarValidadeResultadoPesquisaAsync(Guid id, bool ativo)
    {
        const string sql = @"UPDATE resultado_pesquisa SET ativo = @ativo WHERE id = @id";

        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        dbConnection.Open();

        using var transaction = dbConnection.BeginTransaction();

        var result = await dbConnection.ExecuteAsync(sql.ToString(), new { id, ativo = ativo }, transaction: transaction);

        transaction.Commit();

        return result;
    }

    #endregion

    public async Task<IEnumerable<DicionarioEntity>> ObterPalavrasAsync(EmpresaPalavra idsPayload)
    {
        const string sql = @"SELECT id, empresa_id AS EmpresaId, filial_id AS FilialId, palavra, ativo 
                        FROM dicionario_palavra 
                        WHERE empresa_id = @EmpresaId 
                        AND filial_id = @FilialId 
                        AND ativo = @Ativo 
                        ORDER BY palavra";

        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        dbConnection.Open();

        var result = await dbConnection.QueryAsync<DicionarioEntity>(sql, new
        {
            EmpresaId = idsPayload.EmpresaId,
            FilialId = idsPayload.FilialId,
            Ativo = idsPayload.Ativo
        });

        return result.Any() ? result : [];
    }

    public async Task<int> InserirPaginaDocumento(List<ArquivoPaginaEntity> documentoList)
    {
        const string sql = @"
            INSERT INTO arquivo_pagina (id, arquivo_id, numero_pagina, conteudo_texto, data_criacao)
            VALUES (@Id, @ArquivoId, @NumeroPagina, @ConteudoTexto, @DataCriacao)";

        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        dbConnection.Open();

        using var transaction = dbConnection.BeginTransaction();
        try
        {
            int linhasInseridas = await dbConnection.ExecuteAsync(sql, documentoList, transaction: transaction);
            transaction.Commit();
            return linhasInseridas;
        }
        catch (Exception e)
        {
            log.LogError($"[InserirPaginaDocumento] Erro: {e.Message}", e);
            transaction.Rollback();
            return 0;
        }
    }

    public async Task<ArquivoPaginaEntity?> BuscarDocumentoPorIdAsync(int id)
    {
        const string sql = @"SELECT 
                                Id, 
                                numero_pagina AS NumeroPagina, 
                                data_criacao AS DataCriacao, 
                                data_atualizacao AS DataAtualizacao, 
                                arquivo_id AS ArquivoId 
                            FROM arquivo_pagina WHERE id = @id";

        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        dbConnection.Open();

        using var transaction = dbConnection.BeginTransaction();
        var result = await dbConnection.QueryFirstOrDefaultAsync<ArquivoPaginaEntity>(sql, new { id }, transaction: transaction);
        transaction.Commit();

        return result ?? null;
    }

    public async Task<bool> ApagarArquivoPaginaPorArquivoIdAsync(Guid arquivoId)
    {
        const string sql = @"DELETE FROM arquivo_pagina WHERE arquivo_id = @arquivoId";

        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        dbConnection.Open();

        using var transaction = dbConnection.BeginTransaction();
        var result = await dbConnection.ExecuteAsync(sql, new { arquivoId }, transaction: transaction);
        transaction.Commit();

        return result > 0;
    }

    #region Tabela Arquivo

    public async Task<ArquivoEntity?> ObterArquivoPorIdAsync(Guid id)
    {
        const string sql = @"SELECT 
                                id,
                                empresa_id AS EmpresaId,
                                filial_id AS FilialId,
                                pasta_virtual_id AS PastaVirtualId,
                                estado_arquivo_id AS EstadoArquivoId,
                                nome_original AS NomeOriginal,
                                nome_copia AS NomeCopia,
                                caminho_fisico AS CaminhoFisico,
                                tipo_arquivo AS TipoArquivo,
                                tamanho_bytes AS TamanhoBytes,
                                data_upload AS DataUpload,
                                data_atualizacao AS DataAtualizacao,
                                data_processamento AS DataProcessamento
                            FROM Arquivo WHERE id=@id";

        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        dbConnection.Open();

        //using var transaction = dbConnection.BeginTransaction();
        var result = await dbConnection.QueryFirstOrDefaultAsync<ArquivoEntity>(sql, new { id = id });
        //transaction.Commit();

        return result;
    }

    public async Task<int> InserirArquivoAsync(List<ArquivoEntity> arquivosNovos)
    {
        string sql = "";

        if (arquivosNovos.First().PastaVirtualId == Guid.Empty || string.IsNullOrEmpty(arquivosNovos.First().NomeCopia))
        {
            sql = @"INSERT INTO Arquivo (id, empresa_id, filial_id, estado_arquivo_id, nome_original, caminho_fisico, tipo_arquivo, tamanho_bytes, data_processamento)
                             VALUES (@Id, @EmpresaId, @FilialId, @EstadoArquivoId, @NomeOriginal, @CaminhoFisico, @TipoArquivo, @TamanhoBytes, @DataProcessamento)";
        }
        else
        {
            sql = @"INSERT INTO Arquivo (id, empresa_id, filial_id, pasta_virtual_id, estado_arquivo_id, nome_original, nome_copia, caminho_fisico, tipo_arquivo, tamanho_bytes, data_upload, data_atualizacao, data_processamento)
                             VALUES (@Id, @EmpresaId, @FilialId, @PastaVirtualId, @EstadoArquivoId, @NomeOriginal, @NomeCopia, @CaminhoFisico, @TipoArquivo, @TamanhoBytes, @DataUpload, @DataAtualizacao, @DataProcessamento)";
        }
        
        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        dbConnection.Open();

        using var transaction = dbConnection.BeginTransaction();
        try
        {
            int linhasInseridas = await dbConnection.ExecuteAsync(sql, arquivosNovos, transaction: transaction);
            transaction.Commit();
            return linhasInseridas;
        }
        catch (Exception e)
        {
            log.LogError($"[InserirArquivoAsync] Erro: {e.Message}", e);
            transaction.Rollback();
            return 0;
        }
    }

    public async Task<IEnumerable<ArquivoEntity>?> ObterArquivoPorStatusAsync(EstadoArquivo status)
    {
        const string sql =  @"SELECT id,
                              empresa_id AS EmpresaId, 
                              filial_id AS FilialId, 
                              pasta_virtual_id AS PastaVirtualId, 
                              estado_arquivo_id AS EstadoArquivoId, 
                              nome_original AS NomeOriginal, 
                              nome_copia AS NomeCopia, 
                              caminho_fisico AS CaminhoFisico, 
                              tipo_arquivo AS TipoArquivo, 
                              tamanho_bytes AS TamanhoBytes, 
                              data_upload AS DataUpload, 
                              data_atualizacao AS DataAtualizacao, 
                              data_processamento AS DataProcessamento 
                            FROM arquivo 
                            WHERE estado_arquivo_id = @status";

        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        dbConnection.Open();
        
        var result = await dbConnection.QueryAsync<ArquivoEntity>(sql, new { status });
        
        return !result.Any() ? Enumerable.Empty<ArquivoEntity>() : result;
    }

    public async Task<int> AtualizarArquivoAsync(ArquivoEntity arquivo)
    {
        const string sql = @"UPDATE arquivo
                                 SET
                                        empresa_id = @EmpresaId,
                                        filial_id = @FilialId,
                                        pasta_virtual_id = @PastaVirtualId,
                                        estado_arquivo_id = @EstadoArquivoId,
                                        nome_original = @NomeOriginal,
                                        nome_copia = @NomeCopia,
                                        caminho_fisico = @CaminhoFisico,
                                        tipo_arquivo = @TipoArquivo,
                                        tamanho_bytes = @TamanhoBytes,
                                        data_upload = @DataUpload,
                                        data_atualizacao = @DataAtualizacao,
                                        data_processamento = @DataProcessamento
                                 WHERE Id = @Id;";
        
        var parametros = new
        {
            arquivo.Id,
            arquivo.EmpresaId,
            arquivo.FilialId,
            arquivo.PastaVirtualId,
            arquivo.EstadoArquivoId,
            arquivo.NomeOriginal,
            arquivo.NomeCopia,
            arquivo.CaminhoFisico,
            arquivo.TipoArquivo,
            arquivo.TamanhoBytes,
            arquivo.DataUpload,
            arquivo.DataAtualizacao,
            arquivo.DataProcessamento
        };

        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
  
        dbConnection.Open();

        using var transaction = dbConnection.BeginTransaction();
        
        try
        {
            var result = await dbConnection.ExecuteAsync(sql, parametros, transaction: transaction);

            transaction.Commit();

            return result;
        }
        catch (Exception e)
        {
            log.LogError($"[AtualizarArquivoAsync-Erro] Erro: {e.Message}", e);
            transaction.Rollback();
            return 0;
        }
    }

    public async Task<int> AtualizarArquivosAsync(List<ArquivoEntity> arquivoList)
    {
        if (!arquivoList.Any())
        {
            return 0;
        }

        var sql = new StringBuilder();
        var parametros = new DynamicParameters();

        var i = 0;
        foreach (var arquivo in arquivoList)
        {
            sql.AppendLine($@"UPDATE Arquivo 
                          SET
                                empresa_id = @EmpresaId{i},
                                filial_id = @FilialId{i},
                                pasta_virtual_id = @PastaVirtualId{i},
                                estado_arquivo_id = @EstadoArquivoId{i},
                                nome_original = @NomeOriginal{i},
                                nome_copia = @NomeCopia{i},
                                caminho_fisico = @CaminhoFisico{i},
                                tipo_arquivo = @TipoArquivo{i},
                                tamanho_bytes = @TamanhoBytes{i},
                                data_upload = @DataUpload{i},
                                data_atualizacao = @DataAtualizacao{i},
                                data_processamento = @DataProcessamento{i}
                         WHERE Id = @Id{i};");

            parametros.Add($"@EmpresaId{i}", arquivo.EmpresaId);
            parametros.Add($"@FilialId{i}", arquivo.FilialId);
            parametros.Add($"@PastaVirtualId{i}", arquivo.PastaVirtualId);
            parametros.Add($"@EstadoArquivoId{i}", arquivo.EstadoArquivoId);
            parametros.Add($"@NomeOriginal{i}", arquivo.NomeOriginal);
            parametros.Add($"@NomeCopia{i}", arquivo.NomeCopia);
            parametros.Add($"@CaminhoFisico{i}", arquivo.CaminhoFisico);
            parametros.Add($"@TipoArquivo{i}", arquivo.TipoArquivo);
            parametros.Add($"@TamanhoBytes{i}", arquivo.TamanhoBytes);
            parametros.Add($"@DataUpload{i}", arquivo.DataUpload);
            parametros.Add($"@DataAtualizacao{i}", arquivo.DataAtualizacao);
            parametros.Add($"@DataProcessamento{i}", arquivo.DataProcessamento);
            parametros.Add($"@Id{i}", arquivo.Id);
            
            i++;
        }

        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        dbConnection.Open();

        using var transaction = dbConnection.BeginTransaction();

        var result = await dbConnection.ExecuteAsync(sql.ToString(), parametros, transaction: transaction);

        transaction.Commit();

        return result;
    }

    public async Task<IEnumerable<ArquivoEntity>> ObterTodosDocumentosAsync(EmpresaFilialIdsPayload idsPayload)
    {
        const string sql = @"SELECT id AS Id, 
                                    empresa_id AS EmpresaId , 
                                    filial_id AS FilialId, 
                                    pasta_virtual_id AS PastaVirtualId, 
                                    estado_arquivo_id AS EstadoArquivoId, 
                                    nome_original AS NomeOriginal, 
                                    nome_copia AS NomeCopia, 
                                    caminho_fisico AS CaminhoFisico, 
                                    tipo_arquivo AS TipoArquivo, 
                                    tamanho_bytes AS TamanhoBytes, 
                                    data_upload AS DataUpload, 
                                    data_atualizacao AS DataAtualizacao, 
                                    data_processamento AS DataProcessamento
                            FROM arquivo
                            WHERE empresa_id = @EmpresaId AND filial_id = @FilialId";

        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        dbConnection.Open();

        var result = await dbConnection.QueryAsync<ArquivoEntity>(sql, new
        {
            EmpresaId = idsPayload.EmpresaId,
            FilialId = idsPayload.FilialId
        });

        return !result.Any() ? [] : result;
    }

    public async Task<ArquivoEntity?> BuscarArquivosProcessadosPorLocalEoNome(string caminhoFisico, string nomeOriginal)
    {
        const string sql = @"SELECT 
                                Id,
                                estado_arquivo_id AS EstadoArquivoId,
                                empresa_id AS EmpresaId,
                                filial_id AS FilialId
                            FROM arquivo 
                            WHERE caminho_fisico = @caminhoFisico 
                            AND nome_original = @nomeOriginal
                            AND estado_arquivo_id = @estadoProcessado";

        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        dbConnection.Open();

        using var transaction = dbConnection.BeginTransaction();
        var result = await dbConnection.QuerySingleOrDefaultAsync<ArquivoEntity>(
            sql, 
            new
            {
                caminhoFisico, 
                nomeOriginal,
                estadoProcessado = EstadoArquivo.TextoDaPaginaExtraido
            }, 
            transaction: transaction);
        
        transaction.Commit();

        return result;
    }

    public async Task<ArquivoEntity?> BuscarArquivoProcessadoPorIdentificacao(string nomeOriginal, long tamanhoBytes)
    {
        const string sql = @"SELECT 
                            Id,
                            estado_arquivo_id AS EstadoArquivoId,
                            empresa_id AS EmpresaId,
                            filial_id AS FilialId,
                            nome_original AS NomeOriginal
                        FROM arquivo 
                        WHERE nome_original = @nomeOriginal 
                        AND tamanho_bytes = @tamanhoBytes
                        AND estado_arquivo_id = @estadoProcessado";

        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        dbConnection.Open();
        
        var results = await dbConnection.QueryAsync<ArquivoEntity>(
            sql, 
            new { 
                nomeOriginal, 
                tamanhoBytes,
                estadoProcessado = EstadoArquivo.TextoDaPaginaExtraido
            });
    
        var arquivos = results.ToList();
    
        if (arquivos.Count > 1)
        {
            log.LogWarning($"[BuscarArquivoProcessadoPorIdentificacao] - Encontrados {arquivos.Count} registros para o arquivo {nomeOriginal}. " +
                           $"IDs encontrados: {string.Join(", ", arquivos.Select(a => a.Id))}");
        }

        return arquivos.OrderByDescending(a => a.DataProcessamento).FirstOrDefault();
    }
    
    #endregion
    
    #region tbl Dicionario
    
    public async Task<int> InserirPalavraDicionarioAsync(List<DicionarioEntity> dicionario)
    {
        var sql = @"INSERT INTO 
                        dicionario_palavra (id, empresa_id, filial_id, palavra, ativo)
                    VALUES (@Id, @EmpresaId, @FilialId, @Palavra, @Ativo)";
        
        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        dbConnection.Open();

        using var transaction = dbConnection.BeginTransaction();
        try
        {
            int linhasInseridas = await dbConnection.ExecuteAsync(sql, dicionario, transaction: transaction);
            transaction.Commit();
            return linhasInseridas;
        }
        catch (Exception e)
        {
            log.LogError($"[InserirArquivoAsync] Erro: {e.Message}", e);
            transaction.Rollback();
            return 0;
        }
    }

    public Task<bool> InativarPalavraDicionarioAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> AtualizarPalavraDicionarioAsync()
    {
        throw new NotImplementedException();
    }
    
    #endregion
}

//// EXEMPLO DE PESQUISA
///
//SELECT Id, Titulo, NumeroPagina, Conteudo, LocalArquivo, NomeArquivo, NomeArquivo, MATCH(Conteudo) AGAINST ('+palavras +exemplo' IN BOOLEAN MODE) AS relevancia
// FROM documento
// WHERE MATCH(Conteudo) AGAINST ('+palavras +exemplo' IN BOOLEAN MODE)
// ORDER BY relevancia DESC;
//
// SELECT Id, Titulo, NumeroPagina, Conteudo, LocalArquivo, NomeArquivo, NomeArquivo, MATCH(Conteudo) AGAINST ('palavras exemplo' IN NATURAL LANGUAGE MODE) AS relevancia
// FROM documento
// WHERE MATCH(Conteudo) AGAINST ('palavras exemplo' IN NATURAL LANGUAGE MODE)
// ORDER BY relevancia DESC;
//
// SELECT Id, Titulo, NumeroPagina, Conteudo, LocalArquivo, NomeArquivo, NomeArquivo, MATCH(Conteudo) AGAINST ('palavras exemplo'  WITH QUERY EXPANSION) AS relevancia
// FROM documento
// WHERE MATCH(Conteudo) AGAINST ('palavras exemplo'  WITH QUERY EXPANSION)
// ORDER BY relevancia DESC;
///
///
/// 