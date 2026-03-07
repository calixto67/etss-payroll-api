using PayrollApi.Domain.Entities;

namespace PayrollApi.Domain.Interfaces.Repositories;

public interface IEmployeeDocumentRepository : IBaseRepository<EmployeeDocument>
{
    Task<IEnumerable<EmployeeDocument>> GetByEmployeeIdAsync(
        int employeeId, CancellationToken cancellationToken = default);
}
