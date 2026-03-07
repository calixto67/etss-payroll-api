using Microsoft.EntityFrameworkCore;
using PayrollApi.Domain.Entities;
using PayrollApi.Domain.Interfaces.Repositories;
using PayrollApi.Infrastructure.Data;

namespace PayrollApi.Infrastructure.Repositories;

public class EmergencyContactRepository : BaseRepository<EmployeeEmergencyContact>, IEmergencyContactRepository
{
    public EmergencyContactRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<EmployeeEmergencyContact>> GetByEmployeeIdAsync(
        int employeeId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .AsNoTracking()
            .Where(c => c.EmployeeId == employeeId)
            .OrderByDescending(c => c.IsPrimary)
            .ThenBy(c => c.ContactName)
            .ToListAsync(cancellationToken);
}
