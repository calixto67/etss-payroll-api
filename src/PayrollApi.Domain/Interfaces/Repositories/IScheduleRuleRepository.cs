using PayrollApi.Domain.Entities;

namespace PayrollApi.Domain.Interfaces.Repositories;

public interface IScheduleRuleRepository : IBaseRepository<ScheduleRule>
{
    Task<ScheduleRule> GetRuleAsync(CancellationToken cancellationToken = default);
}
