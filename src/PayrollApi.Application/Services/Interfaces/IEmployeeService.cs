using PayrollApi.Application.Common.Models;
using PayrollApi.Application.DTOs.Employee;

namespace PayrollApi.Application.Services.Interfaces;

public interface IEmployeeService
{
    Task<PagedResult<EmployeeDto>> GetPagedAsync(PaginationParams pagination, int? departmentId = null, CancellationToken cancellationToken = default);
    Task<EmployeeDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<EmployeeDto> GetByEmployeeCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task<EmployeeDto> UpdateAsync(int id, UpdateEmployeeDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string deletedBy, CancellationToken cancellationToken = default);
}
