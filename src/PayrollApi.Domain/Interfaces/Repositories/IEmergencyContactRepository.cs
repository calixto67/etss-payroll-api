using PayrollApi.Domain.Entities;

namespace PayrollApi.Domain.Interfaces.Repositories;

public interface IEmergencyContactRepository : IBaseRepository<EmployeeEmergencyContact>
{
    Task<IEnumerable<EmployeeEmergencyContact>> GetByEmployeeIdAsync(
        int employeeId, CancellationToken cancellationToken = default);
}
