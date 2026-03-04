using PayrollApi.Domain.Interfaces.Repositories;

namespace PayrollApi.Domain.Interfaces;

/// <summary>
/// Unit of Work: groups repository operations into a single transaction boundary.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IEmployeeRepository Employees { get; }
    IPayrollRepository PayrollRecords { get; }

    Task<int> CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
}
