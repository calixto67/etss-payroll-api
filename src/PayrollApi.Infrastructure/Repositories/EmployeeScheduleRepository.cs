using Microsoft.EntityFrameworkCore;
using PayrollApi.Domain.Entities;
using PayrollApi.Domain.Interfaces.Repositories;
using PayrollApi.Infrastructure.Data;

namespace PayrollApi.Infrastructure.Repositories;

public class EmployeeScheduleRepository : BaseRepository<EmployeeSchedule>, IEmployeeScheduleRepository
{
    public EmployeeScheduleRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<EmployeeSchedule>> GetByEmployeeAsync(int employeeId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .Include(es => es.WorkSchedule)
            .Where(es => es.EmployeeId == employeeId)
            .OrderByDescending(es => es.EffectiveDate)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<EmployeeSchedule>> GetByScheduleAsync(int scheduleId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .Include(es => es.Employee)
            .Where(es => es.WorkScheduleId == scheduleId && es.EndDate == null)
            .OrderBy(es => es.Employee.LastName)
            .ToListAsync(cancellationToken);
}
