using Microsoft.EntityFrameworkCore.Storage;
using PayrollApi.Domain.Interfaces;
using PayrollApi.Domain.Interfaces.Repositories;
using PayrollApi.Infrastructure.Data;

namespace PayrollApi.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    public IEmployeeRepository Employees { get; }
    public IPayrollRepository PayrollRecords { get; }

    public UnitOfWork(AppDbContext context, IEmployeeRepository employees, IPayrollRepository payrollRecords)
    {
        _context = context;
        Employees = employees;
        PayrollRecords = payrollRecords;
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default) =>
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        var result = await _context.SaveChangesAsync(cancellationToken);
        if (_transaction is not null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
        return result;
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
