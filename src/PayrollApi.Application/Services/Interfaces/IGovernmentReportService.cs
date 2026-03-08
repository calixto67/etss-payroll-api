using PayrollApi.Application.DTOs.Reports;

namespace PayrollApi.Application.Services.Interfaces;

public interface IGovernmentReportService
{
    Task<IEnumerable<SssReportRowDto>> GetSssReportAsync(int periodId, int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default);
    Task<IEnumerable<PhilHealthReportRowDto>> GetPhilHealthReportAsync(int periodId, int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default);
    Task<IEnumerable<PagIbigReportRowDto>> GetPagIbigReportAsync(int periodId, int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default);
    Task<IEnumerable<BirReportRowDto>> GetBirReportAsync(int periodId, int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default);
}
