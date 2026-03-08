namespace PayrollApi.Domain.Interfaces;

/// <summary>
/// Abstraction for executing stored procedures via Dapper.
/// All business logic and queries go through stored procedures.
/// </summary>
public interface ISqlExecutor
{
    /// <summary>Execute a stored procedure that returns a collection of T.</summary>
    Task<IEnumerable<T>> QueryAsync<T>(string storedProcedure, object? param = null, CancellationToken ct = default);

    /// <summary>Execute a stored procedure that returns a single row (or null).</summary>
    Task<T?> QueryFirstOrDefaultAsync<T>(string storedProcedure, object? param = null, CancellationToken ct = default);

    /// <summary>Execute a stored procedure that returns affected row count.</summary>
    Task<int> ExecuteAsync(string storedProcedure, object? param = null, CancellationToken ct = default);

    /// <summary>Execute a stored procedure that returns a scalar value.</summary>
    Task<T?> ExecuteScalarAsync<T>(string storedProcedure, object? param = null, CancellationToken ct = default);

    /// <summary>Execute a paged stored procedure (result set 1 = data, result set 2 = total count).</summary>
    Task<(IEnumerable<T> Items, int TotalCount)> QueryPagedAsync<T>(string storedProcedure, object? param = null, CancellationToken ct = default);
}
