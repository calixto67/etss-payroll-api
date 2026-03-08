using PayrollApi.Application.DTOs.Reports;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class GovernmentReportService : IGovernmentReportService
{
    private const string SP = "sp_GovernmentReport";
    private readonly ISqlExecutor _sql;

    public GovernmentReportService(ISqlExecutor sql) => _sql = sql;

    public async Task<IEnumerable<SssReportRowDto>> GetSssReportAsync(
        int periodId, int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default)
    {
        return await _sql.QueryAsync<SssReportRowDto>(SP, new
        {
            ActionType = "GET_SSS",
            PayrollPeriodId = periodId,
            DepartmentId = departmentId,
            BranchId = branchId,
            EmployeeId = employeeId
        }, ct);
    }

    public async Task<IEnumerable<PhilHealthReportRowDto>> GetPhilHealthReportAsync(
        int periodId, int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default)
    {
        return await _sql.QueryAsync<PhilHealthReportRowDto>(SP, new
        {
            ActionType = "GET_PHILHEALTH",
            PayrollPeriodId = periodId,
            DepartmentId = departmentId,
            BranchId = branchId,
            EmployeeId = employeeId
        }, ct);
    }

    public async Task<IEnumerable<PagIbigReportRowDto>> GetPagIbigReportAsync(
        int periodId, int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default)
    {
        return await _sql.QueryAsync<PagIbigReportRowDto>(SP, new
        {
            ActionType = "GET_PAGIBIG",
            PayrollPeriodId = periodId,
            DepartmentId = departmentId,
            BranchId = branchId,
            EmployeeId = employeeId
        }, ct);
    }

    public async Task<IEnumerable<BirReportRowDto>> GetBirReportAsync(
        int periodId, int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default)
    {
        return await _sql.QueryAsync<BirReportRowDto>(SP, new
        {
            ActionType = "GET_BIR",
            PayrollPeriodId = periodId,
            DepartmentId = departmentId,
            BranchId = branchId,
            EmployeeId = employeeId
        }, ct);
    }
}
