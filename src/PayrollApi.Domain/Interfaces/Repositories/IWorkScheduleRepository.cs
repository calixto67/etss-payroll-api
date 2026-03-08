using PayrollApi.Domain.Entities;

namespace PayrollApi.Domain.Interfaces.Repositories;

public interface IWorkScheduleRepository : IBaseRepository<WorkSchedule>
{
    Task<WorkSchedule?> GetWithDaysAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> IsNameUniqueAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
}
