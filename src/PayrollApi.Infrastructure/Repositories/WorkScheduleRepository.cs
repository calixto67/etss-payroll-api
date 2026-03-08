using Microsoft.EntityFrameworkCore;
using PayrollApi.Domain.Entities;
using PayrollApi.Domain.Interfaces.Repositories;
using PayrollApi.Infrastructure.Data;

namespace PayrollApi.Infrastructure.Repositories;

public class WorkScheduleRepository : BaseRepository<WorkSchedule>, IWorkScheduleRepository
{
    public WorkScheduleRepository(AppDbContext context) : base(context) { }

    public async Task<WorkSchedule?> GetWithDaysAsync(int id, CancellationToken cancellationToken = default) =>
        await _dbSet
            .Include(w => w.Days.OrderBy(d => d.DayOfWeek))
            .Include(w => w.EmployeeSchedules)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

    public async Task<bool> IsNameUniqueAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default) =>
        !await _dbSet.AnyAsync(w =>
            w.Name == name && (!excludeId.HasValue || w.Id != excludeId.Value),
            cancellationToken);
}
