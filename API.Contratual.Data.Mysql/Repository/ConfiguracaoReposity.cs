using System.Data;
using System.Net;
using API.Contratual.CrossCutting.Notificador;
using API.Contratual.Data.Mysql.Connections;
using API.Contratual.Domain.Configuracao.Entity;
using API.Contratual.Domain.Entity;
using API.Contratual.Domain.Interface.Repository;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace API.Contratual.Data.Mysql.Repository;

public class ConfiguracaoReposity(
    IOptions<ConnectionStrings> appSettings, 
    ILogger<DocumentoRepository> log,
    INotificador notificador) : IConfiguracaoReposity
{
    #region Configuração de pasta

    public async Task<bool> InserirConfiguracaoPastaAsync(ConfiguracaoPastaEntity configuracaoPasta)
    {
        const string sqlInsert = @"INSERT INTO configuracao_pasta (id, empresa_id, filial_id, pasta_origem, pasta_destino, criar_copia, ativo)
                                   VALUES (@Id, @EmpresaId, @FilialId, @PastaOrigem, @PastaDestino, @CriarCopia, @Ativo)";
        
        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        dbConnection.Open();
        
        using var transaction = dbConnection.BeginTransaction();
        
        try
        {
            var res = await dbConnection.ExecuteAsync(sqlInsert, configuracaoPasta, transaction: transaction);
            transaction.Commit();
            return res > 0;
        }
        catch (MySqlException e)
        {
            log.LogError($"[InserirConfiguracaoPastaAsync] Erro: {e.Message}", e);
            
            notificador.Handle(new Notificacao("Ocorreu um problema",
                $"Tentativa de inserir configuração de pasta falhou",
                TipoNotificacao.Erro,
                HttpStatusCode.InternalServerError));
            
            transaction.Rollback();
            return false;
        }
    }
    
    public async Task<bool> AtualizarConfiguracaoPastaAsync(ConfiguracaoPastaEntity configuracaoPasta)
    {
        const string sql = @"UPDATE configuracao_pasta
                                SET
                                    pasta_origem = @PastaOrigem, 
                                    pasta_destino = @PastaDestino, 
                                    criar_copia = @CriarCopia, 
                                    ativo = @Ativo, 
                                    data_atualizacao = @DataAtualizacao 
                                WHERE id = @Id";
        
        var parameters = new
        {
            configuracaoPasta.Id,
            configuracaoPasta.PastaOrigem,
            configuracaoPasta.PastaDestino,
            configuracaoPasta.CriarCopia,
            configuracaoPasta.Ativo,
            configuracaoPasta.DataAtualizacao
        };
        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        dbConnection.Open();
        
        using var transaction = dbConnection.BeginTransaction();

        try
        {
            var result = await dbConnection.ExecuteAsync(sql, parameters, transaction: transaction);
            transaction.Commit();

            return result > 0;
        }
        catch (Exception e)
        {
            log.LogError($"[AtualizarConfiguracaoPastaAsync-Erro] Erro: {e.Message}", e);
            notificador.Handle(new Notificacao("Ocorreu um problema",
                $"Tentativa de atualizar configuração de pasta falhou",
                TipoNotificacao.Erro,
                HttpStatusCode.InternalServerError));
                
            transaction.Rollback();
            return false;
        }
    }
    
    public async Task<ConfiguracaoPastaEntity> ObterConfiguracaoPastaPorEmpresaIdFilialIdAsync(Guid empresaId, Guid filialId)
    {
        const string sql = @"SELECT 
                                id, 
                                empresa_id as EmpresaId, 
                                filial_id as FilialId, 
                                pasta_origem as PastaOrigem, 
                                pasta_destino as PastaDestino, 
                                criar_copia as CriarCopia, 
                                ativo, 
                                data_cadastro as DataCadastro, 
                                data_atualizacao as DataAtualizacao 
                             FROM configuracao_pasta WHERE empresa_id = @empresaId AND filial_id = @filialId";
        
        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        dbConnection.Open();
        
        using var transaction = dbConnection.BeginTransaction();
        var result = await dbConnection.QueryFirstOrDefaultAsync<ConfiguracaoPastaEntity>(sql, new { empresaId, filialId }, transaction: transaction);
        transaction.Commit();
        
        return result;
    }
    
    public async Task<ConfiguracaoPastaEntity?> ObterConfiguracaoPastaPorIdAsync(Guid id)
    {
        const string sql = @"SELECT 
                                id, 
                                empresa_id as EmpresaId, 
                                filial_id as FilialId, 
                                pasta_origem as PastaOrigem, 
                                pasta_destino as PastaDestino, 
                                criar_copia as CriarCopia, 
                                ativo, 
                                data_cadastro as DataCadastro, 
                                data_atualizacao as DataAtualizacao 
                             FROM configuracao_pasta WHERE id = @id";
        
        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        dbConnection.Open();
        
        using var transaction = dbConnection.BeginTransaction();
        var result = await dbConnection.QueryFirstOrDefaultAsync<ConfiguracaoPastaEntity>(sql, new { id }, transaction: transaction);
        transaction.Commit();

        return result;
    }
    
    #endregion
    
    #region Configuração

    public async Task<bool> InserirConfiguracaoAsync(ConfiguracaoEntity configuracao)
    {
        const string sqlInsert = @"INSERT INTO Configuracao (id, empresa_id, filial_id, chave, valor, descricao, tipo_valor, ativo)
                                   VALUES (@Id, @EmpresaId, @FilialId, @Chave, @Valor, @Descricao, @TipoValor, @Ativo)";
        
        
        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        dbConnection.Open();
        
        using var transaction = dbConnection.BeginTransaction();
        
        try
        {
            var res = await dbConnection.ExecuteAsync(sqlInsert, configuracao, transaction: transaction);
            transaction.Commit();
            return res > 0;
        }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            transaction.Rollback();
            var errorMessage = ex.Message.ToLower();

            if (errorMessage.Contains("uk_configuracao_empresa_filial_chave"))
            {
                log.LogError($"Tentativa de inserir empresa e filial duplicado: empresaId {configuracao.EmpresaId}, filialId {configuracao.FilialId}");
          
                notificador.Handle(new Notificacao("Ocorreu um problema",
                    $"Tentativa de inserir chaves duplicadas",
                    TipoNotificacao.Alerta,
                    HttpStatusCode.Conflict));
                
                return false;
            }
            
            log.LogError($"Erro de duplicidade não mapeado: {ex.Message}", ex);
            
            notificador.Handle(new Notificacao("Ocorreu um problema",
                $"Tentativa de inserir empresa falou",
                TipoNotificacao.Alerta,
                HttpStatusCode.Conflict));
            
            return false;
        }
        catch (Exception e)
        {
            log.LogError($"[InserirEmpresaAsync] Erro: {e.Message}", e);
            
            notificador.Handle(new Notificacao("Ocorreu um problema",
                $"Tentativa de inserir configuração falhou",
                TipoNotificacao.Erro,
                HttpStatusCode.InternalServerError));
            
            transaction.Rollback();
            return false;
        }
    }

    public async Task<bool> AtualizarConfiguracaoAsync(ConfiguracaoEntity configuracao)
    {
        const string sql = @"UPDATE Configuracao
                                SET
                                    valor = @Valor, 
                                    descricao = @Descricao, 
                                    tipo_valor = @TipoValor, 
                                    ativo = @Ativo, 
                                    data_atualizacao = @DataAtualizacao 
                                WHERE id = @Id";
        
        var parameters = new
        {
            configuracao.Id,
            configuracao.Valor,
            configuracao.Descricao,
            configuracao.TipoValor,
            configuracao.Ativo,
            configuracao.DataAtualizacao
        };
        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        dbConnection.Open();
        
        using var transaction = dbConnection.BeginTransaction();

        try
        {
            var result = await dbConnection.ExecuteAsync(sql, parameters, transaction: transaction);
            transaction.Commit();

            return result > 0;
        }
        catch (Exception e)
        {
            log.LogError($"[EditarEmpresaAsync-Erro] Erro: {e.Message}", e);
            notificador.Handle(new Notificacao("Ocorreu um problema",
                $"Tentativa de atualizar falhou",
                TipoNotificacao.Erro,
                HttpStatusCode.InternalServerError));
                
            transaction.Rollback();
            return false;
        }
    }

    public async Task<ConfiguracaoEntity?> ObterConfiguracaoPorIdAsync(Guid id)
    {
        const string sql = @"SELECT 
                                id, 
                                empresa_id as EmpresaId, 
                                filial_id as FilialId, 
                                chave, 
                                valor, 
                                descricao, 
                                tipo_valor as TipoValor, 
                                ativo, 
                                data_cadastro as DataCadastro, 
                                data_atualizacao as DataAtualizacao 
                             FROM Configuracao WHERE id = @id";
        
        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        dbConnection.Open();
        
        using var transaction = dbConnection.BeginTransaction();
        var result = await dbConnection.QueryFirstOrDefaultAsync<ConfiguracaoEntity>(sql, new { id }, transaction: transaction);
        transaction.Commit();

        return result;
    }

    public async Task<IEnumerable<ConfiguracaoEntity>> ObterConfiguracaoPorEmpresaIdAsync(Guid empresaId)
    {
        const string sql = @"SELECT 
                                id, 
                                empresa_id as EmpresaId, 
                                filial_id as FilialId, 
                                chave, 
                                valor, 
                                descricao, 
                                tipo_valor as TipoValor, 
                                ativo, 
                                data_cadastro as DataCadastro, 
                                data_atualizacao as DataAtualizacao 
                             FROM Configuracao WHERE empresa_id = @empresaId";
        
        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        dbConnection.Open();
        
        using var transaction = dbConnection.BeginTransaction();
        var result = await dbConnection.QueryAsync<ConfiguracaoEntity>(sql, new { empresaId }, transaction: transaction);
        
        transaction.Commit();

        return !result.Any() ? [] : result;
    }

    #endregion
    
}