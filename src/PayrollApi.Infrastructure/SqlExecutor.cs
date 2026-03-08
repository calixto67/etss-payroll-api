using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using PayrollApi.Domain.Interfaces;
using PayrollApi.Infrastructure.Data;

namespace PayrollApi.Infrastructure;

/// <summary>
/// Dapper-based implementation of ISqlExecutor.
/// Uses the EF Core DbContext's underlying connection for consistency.
/// </summary>
public class SqlExecutor : ISqlExecutor
{
    private readonly AppDbContext _context;

    public SqlExecutor(AppDbContext context) => _context = context;

    public async Task<IEnumerable<T>> QueryAsync<T>(string storedProcedure, object? param = null, CancellationToken ct = default)
    {
        var connection = await GetOpenConnectionAsync(ct);
        return await connection.QueryAsync<T>(
            new CommandDefinition(storedProcedure, param, commandType: CommandType.StoredProcedure, cancellationToken: ct));
    }

    public async Task<T?> QueryFirstOrDefaultAsync<T>(string storedProcedure, object? param = null, CancellationToken ct = default)
    {
        var connection = await GetOpenConnectionAsync(ct);
        return await connection.QueryFirstOrDefaultAsync<T>(
            new CommandDefinition(storedProcedure, param, commandType: CommandType.StoredProcedure, cancellationToken: ct));
    }

    public async Task<int> ExecuteAsync(string storedProcedure, object? param = null, CancellationToken ct = default)
    {
        var connection = await GetOpenConnectionAsync(ct);
        return await connection.ExecuteAsync(
            new CommandDefinition(storedProcedure, param, commandType: CommandType.StoredProcedure, cancellationToken: ct));
    }

    public async Task<T?> ExecuteScalarAsync<T>(string storedProcedure, object? param = null, CancellationToken ct = default)
    {
        var connection = await GetOpenConnectionAsync(ct);
        return await connection.ExecuteScalarAsync<T>(
            new CommandDefinition(storedProcedure, param, commandType: CommandType.StoredProcedure, cancellationToken: ct));
    }

    public async Task<(IEnumerable<T> Items, int TotalCount)> QueryPagedAsync<T>(string storedProcedure, object? param = null, CancellationToken ct = default)
    {
        var connection = await GetOpenConnectionAsync(ct);
        using var multi = await connection.QueryMultipleAsync(
            new CommandDefinition(storedProcedure, param, commandType: CommandType.StoredProcedure, cancellationToken: ct));

        var items = (await multi.ReadAsync<T>()).ToList();
        var totalCount = await multi.ReadSingleAsync<int>();
        return (items, totalCount);
    }

    private async Task<IDbConnection> GetOpenConnectionAsync(CancellationToken ct)
    {
        var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(ct);
        return connection;
    }
}
