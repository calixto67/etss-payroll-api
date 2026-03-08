using PayrollApi.Application.DTOs.Reports;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class AttendanceReportService : IAttendanceReportService
{
    private const string SP = "sp_AttendanceReport";
    private readonly ISqlExecutor _sql;

    public AttendanceReportService(ISqlExecutor sql) => _sql = sql;

    public async Task<IEnumerable<DailyAttendanceRowDto>> GetDailyAsync(
        int periodId, int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default)
    {
        return await _sql.QueryAsync<DailyAttendanceRowDto>(SP, new
        {
            ActionType = "DAILY",
            PayrollPeriodId = periodId,
            DepartmentId = departmentId,
            BranchId = branchId,
            EmployeeId = employeeId
        }, ct);
    }

    public async Task<IEnumerable<TardinessRowDto>> GetTardinessAsync(
        int periodId, int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default)
    {
        return await _sql.QueryAsync<TardinessRowDto>(SP, new
        {
            ActionType = "TARDINESS",
            PayrollPeriodId = periodId,
            DepartmentId = departmentId,
            BranchId = branchId,
            EmployeeId = employeeId
        }, ct);
    }

    public async Task<IEnumerable<AbsenteeismRowDto>> GetAbsenteeismAsync(
        int periodId, int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default)
    {
        return await _sql.QueryAsync<AbsenteeismRowDto>(SP, new
        {
            ActionType = "ABSENTEEISM",
            PayrollPeriodId = periodId,
            DepartmentId = departmentId,
            BranchId = branchId,
            EmployeeId = employeeId
        }, ct);
    }

    public async Task<IEnumerable<OvertimeRowDto>> GetOvertimeAsync(
        int periodId, int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default)
    {
        return await _sql.QueryAsync<OvertimeRowDto>(SP, new
        {
            ActionType = "OVERTIME",
            PayrollPeriodId = periodId,
            DepartmentId = departmentId,
            BranchId = branchId,
            EmployeeId = employeeId
        }, ct);
    }

    public async Task<IEnumerable<LeaveUsageRowDto>> GetLeaveUsageAsync(
        int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default)
    {
        return await _sql.QueryAsync<LeaveUsageRowDto>(SP, new
        {
            ActionType = "LEAVE_USAGE",
            PayrollPeriodId = (int?)null,
            DepartmentId = departmentId,
            BranchId = branchId,
            EmployeeId = employeeId
        }, ct);
    }

    public async Task<IEnumerable<AttendanceSummaryRowDto>> GetSummaryAsync(
        int periodId, int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default)
    {
        return await _sql.QueryAsync<AttendanceSummaryRowDto>(SP, new
        {
            ActionType = "SUMMARY",
            PayrollPeriodId = periodId,
            DepartmentId = departmentId,
            BranchId = branchId,
            EmployeeId = employeeId
        }, ct);
    }
}
