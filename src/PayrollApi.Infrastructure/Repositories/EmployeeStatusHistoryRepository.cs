using Microsoft.EntityFrameworkCore;
using PayrollApi.Domain.Entities;
using PayrollApi.Domain.Interfaces.Repositories;
using PayrollApi.Infrastructure.Data;

namespace PayrollApi.Infrastructure.Repositories;

public class EmployeeStatusHistoryRepository : BaseRepository<EmployeeStatusHistory>, IEmployeeStatusHistoryRepository
{
    public EmployeeStatusHistoryRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<EmployeeStatusHistory>> GetByEmployeeIdAsync(
        int employeeId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .AsNoTracking()
            .Where(h => h.EmployeeId == employeeId)
            .OrderByDescending(h => h.ChangedAt)
            .ToListAsync(cancellationToken);
}
