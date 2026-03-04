using PayrollApi.Domain.Entities;

namespace PayrollApi.Domain.Interfaces.Repositories;

public interface IEmployeeRepository : IBaseRepository<Employee>
{
    Task<Employee?> GetByEmployeeCodeAsync(string employeeCode, CancellationToken cancellationToken = default);
    Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Employee> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? search, int? departmentId,
        EmploymentStatus? status, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> IsEmployeeCodeUniqueAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default);
}
