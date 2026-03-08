using PayrollApi.Domain.Entities;

namespace PayrollApi.Domain.Interfaces.Repositories;

public interface ISalaryHistoryRepository : IBaseRepository<SalaryHistory>
{
    Task<IEnumerable<SalaryHistory>> GetByEmployeeIdAsync(
        int employeeId, CancellationToken cancellationToken = default);
}
