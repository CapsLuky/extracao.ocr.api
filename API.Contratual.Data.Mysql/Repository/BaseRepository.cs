using MySqlConnector;
using API.Contratual.CrossCutting.Common.Enum;
using API.Contratual.Data.Mysql.Connections;
using Dapper;

namespace API.Contratual.Data.Mysql.Repository;

public class BaseRepository
{
    // private readonly string _connectionString;
    //
    // public BaseRepository(StringConnections connectionStringBanco)
    // {
    //     _connectionString = connectionStringBanco[StringConnectionEnum.Makem];
    // }
    //
    // protected async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param)
    // {
    //     await using var connection = new MySqlConnection(_connectionString);
    //     await connection.OpenAsync();
    //     
    //     await using var transaction = await connection.BeginTransactionAsync();
    //     dynamic result = await connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction);
    //     await transaction.CommitAsync();
    //     
    //     return result;
    // }
    //
    // protected T QueryFirstOrDefault<T>(string sql, object param)
    // {
    //     using var connection = new MySqlConnection(_connectionString);
    //     connection.Open();
    //     
    //     using var transaction = connection.BeginTransaction();
    //     dynamic result = connection.QueryFirstOrDefault<T>(sql, param, transaction);
    //     transaction.Commit();
    //     
    //     return result;
    // }
    //
    // protected async Task<int> ExecuteAsync(string sql, object param)
    // {
    //     await using var connection = new MySqlConnection(_connectionString);
    //     await connection.OpenAsync();
    //
    //     await using var transaction = await connection.BeginTransactionAsync();
    //     dynamic result = await connection.ExecuteAsync(sql, param, transaction);
    //     await transaction.CommitAsync();
    //
    //     return result;
    // }
    //
    // protected int Execute(string sql, object param)
    // {
    //     using var connection = new MySqlConnection(_connectionString);
    //     connection.Open();
    //
    //     using var transaction = connection.BeginTransaction();
    //     dynamic result = connection.Execute(sql, param, transaction);
    //     transaction.Commit();
    //
    //     return result;
    // }
    //
    // protected async Task<T> ExecuteScalarAsync<T>(string sql, object param)
    // {
    //     await using var connection = new MySqlConnection(_connectionString);
    //     await connection.OpenAsync();
    //
    //     await using var transaction = await connection.BeginTransactionAsync();
    //     dynamic result = await connection.ExecuteScalarAsync<T>(sql, param, transaction);
    //     await transaction.CommitAsync();
    //
    //     return result;
    // }
    //
    // protected T ExecuteScalar<T>(string sql, object param)
    // {
    //     using var connection = new MySqlConnection(_connectionString);
    //     connection.Open();
    //
    //     using var transaction = connection.BeginTransaction();
    //     dynamic result = connection.ExecuteScalar<T>(sql, param, transaction);
    //     transaction.Commit();
    //
    //     return result;
    // }
    //
    // protected async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param)
    // {
    //     await using var connection = new MySqlConnection(_connectionString);
    //     await connection.OpenAsync();
    //
    //     await using var transaction = await connection.BeginTransactionAsync();
    //     dynamic result = await connection.QueryAsync<T>(sql, param, transaction);
    //     await transaction.CommitAsync();
    //
    //     return result;
    // }
}