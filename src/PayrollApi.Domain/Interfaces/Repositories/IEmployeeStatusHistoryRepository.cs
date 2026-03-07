using PayrollApi.Domain.Entities;

namespace PayrollApi.Domain.Interfaces.Repositories;

public interface IEmployeeStatusHistoryRepository : IBaseRepository<EmployeeStatusHistory>
{
    Task<IEnumerable<EmployeeStatusHistory>> GetByEmployeeIdAsync(
        int employeeId, CancellationToken cancellationToken = default);
}
