using System.Data;
using System.Net;
using API.Contratual.CrossCutting.Notificador;
using API.Contratual.Data.Mysql.Connections;
using API.Contratual.Domain.Empresa.Entity;
using API.Contratual.Domain.Entity;
using API.Contratual.Domain.Interface.Repository;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace API.Contratual.Data.Mysql.Repository;

public class EmpresaRepository(IOptions<ConnectionStrings> appSettings, 
    ILogger<DocumentoRepository> log,
    INotificador notificador) : IEmpresaRepository
{
    public async Task<bool> InserirEmpresaAsync(EmpresaEntity empresa)
    {
        const string sqlInsert = @"INSERT INTO empresa (id, razao_social, cnpj, ativo, data_atualizacao)
                             VALUES (@Id, @RazaoSocial, @Cnpj, @Ativo, @DataAtualizacao)";
        
        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        dbConnection.Open();
        
        using var transaction = dbConnection.BeginTransaction();

        try
        {
            var res = await dbConnection.ExecuteAsync(sqlInsert, empresa, transaction: transaction);
            transaction.Commit();
            return res > 0;
        }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            transaction.Rollback();
            var errorMessage = ex.Message.ToLower();

            if (errorMessage.Contains("uk_empresas_cnpj"))
            {
                log.LogError($"Tentativa de inserir empresa com CNPJ duplicado: {empresa.Cnpj}");
          
                notificador.Handle(new Notificacao("Ocorreu um problema",
                    $"Tentativa de inserir empresa com CNPJ duplicado: {empresa.Cnpj}",
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
                $"Tentativa de inserir empresa falhou",
                TipoNotificacao.Erro,
                HttpStatusCode.InternalServerError));
            
            transaction.Rollback();
            return false;
        }
    }

    public async Task<EmpresaEntity?> AtualizarEmpresaAsync(EmpresaEntity empresa)
    {
        const string sql = @"UPDATE empresa
                                SET 
                                    razao_social = @RazaoSocial,
                                    cnpj = @Cnpj,
                                    ativo = @Ativo,
                                    data_atualizacao = @DataAtualizacao
                                WHERE id = @Id";
        
        var parameters = new
        {
            empresa.Id,
            empresa.RazaoSocial,
            empresa.Cnpj,
            empresa.Ativo,
            empresa.DataAtualizacao
        };
        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        dbConnection.Open();
        
        using var transaction = dbConnection.BeginTransaction();

        try
        {
            var result = await dbConnection.ExecuteAsync(sql, parameters, transaction: transaction);
            transaction.Commit();

            if (result > 0)
            {
                return await ObterEmpresaPorIdAsync(empresa.Id);
            }

            return null;
        }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            transaction.Rollback();
            var errorMessage = ex.Message.ToLower();

            if (errorMessage.Contains("uk_empresas_cnpj"))
            {
                log.LogError($"Tentativa de inserir empresa com CNPJ duplicado: {empresa.Cnpj}");
          
                notificador.Handle(new Notificacao("Ocorreu um problema",
                    $"Tentativa de inserir empresa com CNPJ duplicado: {empresa.Cnpj}",
                    TipoNotificacao.Alerta,
                    HttpStatusCode.Conflict));
                
                return null;
            }
            
            log.LogError($"Erro de duplicidade não mapeado: {ex.Message}", ex);
            
            notificador.Handle(new Notificacao("Ocorreu um problema",
                $"Tentativa de inserir empresa falou",
                TipoNotificacao.Alerta,
                HttpStatusCode.Conflict));
            
            return null;
        }
        catch (Exception e)
        {
            log.LogError($"[EditarEmpresaAsync-Erro] Erro: {e.Message}", e);
            notificador.Handle(new Notificacao("Ocorreu um problema",
                $"Tentativa de atualizar falhou",
                TipoNotificacao.Erro,
                HttpStatusCode.InternalServerError));
            transaction.Rollback();
            return null;
        }
    }
    
    public async Task<EmpresaEntity?> ObterEmpresaPorIdAsync(Guid id)
    {
        const string sql = "SELECT id, razao_social AS RazaoSocial, cnpj, ativo, data_cadastro AS DataCadastro, data_atualizacao AS DataAtualizacao FROM empresa WHERE id = @id";
        
        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        dbConnection.Open();
        
        using var transaction = dbConnection.BeginTransaction();
        var result = await dbConnection.QueryFirstOrDefaultAsync<EmpresaEntity>(sql, new { id }, transaction: transaction);
        transaction.Commit();

        return result;
    }
    
    public async Task<EmpresaEntity?> ObterEmpresaPorCnpjAsync(string cnpj)
    {
        const string sql = @"SELECT id, razao_social AS RazaoSocial, cnpj, ativo, data_cadastro AS DataCadastro, data_atualizacao AS DataAtualizacao
                              FROM empresa 
                              WHERE cnpj = @cnpj";
        
        await using var connection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();
        var result = await connection.QueryFirstOrDefaultAsync<EmpresaEntity>(sql: sql, param: new { cnpj }, transaction, commandType: CommandType.Text);
        await transaction.CommitAsync();

        return result;
    }

    public async Task<bool> InserirFilialAsync(FilialEntity filialEntity)
    {
        const string sqlInsert = @"INSERT INTO filial (id, empresa_id, nome, cnpj, ativo)
                             VALUES (@Id, @EmpresaId, @Nome, @Cnpj, @Ativo)";
        
        
        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        dbConnection.Open();
        
        using var transaction = dbConnection.BeginTransaction();

        try
        {
            var res = await dbConnection.ExecuteAsync(sqlInsert, filialEntity, transaction: transaction);
            transaction.Commit();
            return res > 0;
        }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            transaction.Rollback();
            var errorMessage = ex.Message.ToLower();

            if (errorMessage.Contains("uk_filiais_cnpj"))
            {
                log.LogError($"Tentativa de inserir com CNPJ duplicado: {filialEntity.Cnpj}");
          
                notificador.Handle(new Notificacao("Ocorreu um problema",
                    $"Tentativa de inserir com CNPJ duplicado: {filialEntity.Cnpj}",
                    TipoNotificacao.Alerta,
                    HttpStatusCode.Conflict));
                
                return false;
            }
            
            log.LogError($"Erro de duplicidade não mapeado: {ex.Message}", ex);
            
            notificador.Handle(new Notificacao("Ocorreu um problema",
                $"Tentativa de inserir falou",
                TipoNotificacao.Alerta,
                HttpStatusCode.Conflict));
            
            return false;
        }
        catch (Exception e)
        {
            log.LogError($"[InserirFilialAsync] Erro: {e.Message}", e);
            
            notificador.Handle(new Notificacao("Ocorreu um problema",
                $"Tentativa de inserir falou",
                TipoNotificacao.Erro,
                HttpStatusCode.Conflict));
            
            transaction.Rollback();
            return false;
        }
    }
    
    public async Task<FilialEntity?> AtualizarFilialAsync(FilialEntity filial)
    {
        const string sql = @"UPDATE filial
                                SET 
                                    nome = @Nome,
                                    cnpj = @Cnpj,
                                    ativo = @Ativo,
                                    data_atualizacao = @DataAtualizacao
                                WHERE id = @Id";
        
        var parameters = new
        {
            filial.Id,
            filial.Nome,
            filial.Cnpj,
            filial.Ativo,
            filial.DataAtualizacao
        };
        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        dbConnection.Open();
        
        using var transaction = dbConnection.BeginTransaction();

        try
        {
            var result = await dbConnection.ExecuteAsync(sql, parameters, transaction: transaction);
            transaction.Commit();

            if (result > 0)
            {
                return await ObterFilialPorIdAsync(filial.Id);
            }

            return null;
        }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            transaction.Rollback();
            var errorMessage = ex.Message.ToLower();

            if (errorMessage.Contains("uk_filiais_cnpj"))
            {
                log.LogError($"Tentativa de inserir um CNPJ duplicado: {filial.Cnpj}");
          
                notificador.Handle(new Notificacao("Ocorreu um problema",
                    $"Tentativa de inserir um CNPJ duplicado: {filial.Cnpj}",
                    TipoNotificacao.Alerta,
                    HttpStatusCode.Conflict));
                
                return null;
            }
            
            log.LogError($"Erro de duplicidade não mapeado: {ex.Message}", ex);
            
            notificador.Handle(new Notificacao("Ocorreu um problema",
                $"Tentativa de inserir empresa falou",
                TipoNotificacao.Alerta,
                HttpStatusCode.Conflict));
            
            return null;
        }
        catch (Exception e)
        {
            log.LogError($"[EditarEmpresaAsync-Erro] Erro: {e.Message}", e);
            notificador.Handle(new Notificacao("Ocorreu um problema",
                $"Tentativa de atualizar falhou",
                TipoNotificacao.Erro,
                HttpStatusCode.InternalServerError));
                
            transaction.Rollback();
            return null;
        }
    }
    
    public async Task<FilialEntity?> ObterFilialPorIdAsync(Guid id)
    {
        const string sql = @"SELECT id, empresa_id AS EmpresaId, nome, cnpj, ativo, data_cadastro AS DataCadastro, data_atualizacao AS DataAtualizacao 
                             FROM filial WHERE id = @id";
        
        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        dbConnection.Open();
        
        using var transaction = dbConnection.BeginTransaction();
        var result = await dbConnection.QueryFirstOrDefaultAsync<FilialEntity>(sql, new { id }, transaction: transaction);
        transaction.Commit();

        return result;
    }
    
    public async Task<IEnumerable<FilialEntity>> ObterFiliaisPorEmpresaIdAsync(Guid empresaId)
    {
        const string sql = @"SELECT id, empresa_id AS EmpresaId, nome, cnpj, ativo, data_cadastro AS DataCadastro, data_atualizacao AS DataAtualizacao 
                             FROM filial WHERE empresa_id = @empresaId";
        
        using IDbConnection dbConnection = new MySqlConnection(appSettings.Value.DataBaseObraSafe);
        dbConnection.Open();
        
        using var transaction = dbConnection.BeginTransaction();
        var result = await dbConnection.QueryAsync<FilialEntity>(sql, new { empresaId }, transaction: transaction);
        transaction.Commit();

        var obterFiliaisPorEmpresaIdAsync = result.ToList();
        return obterFiliaisPorEmpresaIdAsync.Count != 0 ? obterFiliaisPorEmpresaIdAsync : [];
    }
}