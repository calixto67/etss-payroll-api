using PayrollApi.Domain.Entities;

namespace PayrollApi.Domain.Interfaces.Repositories;

public interface IEmployeeScheduleRepository : IBaseRepository<EmployeeSchedule>
{
    Task<IEnumerable<EmployeeSchedule>> GetByEmployeeAsync(int employeeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<EmployeeSchedule>> GetByScheduleAsync(int scheduleId, CancellationToken cancellationToken = default);
}
