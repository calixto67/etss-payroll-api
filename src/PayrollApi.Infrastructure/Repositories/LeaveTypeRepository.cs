using Microsoft.EntityFrameworkCore;
using PayrollApi.Domain.Entities;
using PayrollApi.Domain.Interfaces.Repositories;
using PayrollApi.Infrastructure.Data;

namespace PayrollApi.Infrastructure.Repositories;

public class LeaveTypeRepository : BaseRepository<LeaveType>, ILeaveTypeRepository
{
    public LeaveTypeRepository(AppDbContext context) : base(context) { }

    public async Task<bool> IsNameUniqueAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default) =>
        !await _dbSet.AnyAsync(l =>
            l.Name == name && (!excludeId.HasValue || l.Id != excludeId.Value),
            cancellationToken);

    public async Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default) =>
        !await _dbSet.AnyAsync(l =>
            l.Code == code && (!excludeId.HasValue || l.Id != excludeId.Value),
            cancellationToken);

    public async Task<IEnumerable<LeaveType>> GetAllActiveAsync(CancellationToken cancellationToken = default) =>
        await _dbSet
            .Where(l => l.IsActive)
            .OrderBy(l => l.Name)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
}
