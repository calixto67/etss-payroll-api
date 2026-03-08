using Microsoft.EntityFrameworkCore;
using PayrollApi.Domain.Entities;
using PayrollApi.Domain.Interfaces.Repositories;
using PayrollApi.Infrastructure.Data;

namespace PayrollApi.Infrastructure.Repositories;

public class SalaryHistoryRepository : BaseRepository<SalaryHistory>, ISalaryHistoryRepository
{
    public SalaryHistoryRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<SalaryHistory>> GetByEmployeeIdAsync(
        int employeeId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .AsNoTracking()
            .Where(h => h.EmployeeId == employeeId)
            .OrderByDescending(h => h.EffectiveDate)
            .ToListAsync(cancellationToken);
}
