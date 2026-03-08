using PayrollApi.Application.DTOs.EmployeeEnrollment;

namespace PayrollApi.Application.Services.Interfaces;

public interface IEmployeeEnrollmentService
{
    // Allowances
    Task<IEnumerable<EmployeeAllowanceDto>> GetAllowancesAsync(int? employeeId = null, string? search = null, int? departmentId = null, int? branchId = null, CancellationToken ct = default);
    Task<EmployeeAllowanceDto> GetAllowanceByIdAsync(int id, CancellationToken ct = default);
    Task<EmployeeAllowanceDto> CreateAllowanceAsync(CreateEmployeeAllowanceDto dto, string createdBy, CancellationToken ct = default);
    Task<EmployeeAllowanceDto> UpdateAllowanceAsync(int id, UpdateEmployeeAllowanceDto dto, string updatedBy, CancellationToken ct = default);
    Task DeleteAllowanceAsync(int id, string deletedBy, CancellationToken ct = default);

    // Deductions
    Task<IEnumerable<EmployeeDeductionDto>> GetDeductionsAsync(int? employeeId = null, string? search = null, int? departmentId = null, int? branchId = null, CancellationToken ct = default);
    Task<EmployeeDeductionDto> GetDeductionByIdAsync(int id, CancellationToken ct = default);
    Task<EmployeeDeductionDto> CreateDeductionAsync(CreateEmployeeDeductionDto dto, string createdBy, CancellationToken ct = default);
    Task<EmployeeDeductionDto> UpdateDeductionAsync(int id, UpdateEmployeeDeductionDto dto, string updatedBy, CancellationToken ct = default);
    Task DeleteDeductionAsync(int id, string deletedBy, CancellationToken ct = default);
}
