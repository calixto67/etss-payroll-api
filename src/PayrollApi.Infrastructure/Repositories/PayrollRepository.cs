using Microsoft.EntityFrameworkCore;
using PayrollApi.Domain.Entities;
using PayrollApi.Domain.Interfaces.Repositories;
using PayrollApi.Infrastructure.Data;

namespace PayrollApi.Infrastructure.Repositories;

public class PayrollRepository : BaseRepository<PayrollRecord>, IPayrollRepository
{
    public PayrollRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<PayrollRecord>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .Include(p => p.PayrollPeriod)
            .Where(p => p.EmployeeId == employeeId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<PayrollRecord>> GetByPeriodIdAsync(int periodId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .Include(p => p.Employee)
            .Where(p => p.PayrollPeriodId == periodId)
            .OrderBy(p => p.Employee!.LastName)
            .ToListAsync(cancellationToken);

    public async Task<PayrollRecord?> GetByEmployeeAndPeriodAsync(int employeeId, int periodId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .Include(p => p.Employee)
            .Include(p => p.PayrollPeriod)
            .FirstOrDefaultAsync(p => p.EmployeeId == employeeId && p.PayrollPeriodId == periodId, cancellationToken);

    public async Task<(IEnumerable<PayrollRecord> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, int? employeeId, int? periodId,
        PayrollStatus? status, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(p => p.Employee)
            .Include(p => p.PayrollPeriod)
            .AsNoTracking();

        if (employeeId.HasValue) query = query.Where(p => p.EmployeeId == employeeId.Value);
        if (periodId.HasValue) query = query.Where(p => p.PayrollPeriodId == periodId.Value);
        if (status.HasValue) query = query.Where(p => p.Status == status.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
