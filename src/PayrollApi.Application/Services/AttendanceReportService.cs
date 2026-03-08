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
        DateTime startDate, DateTime endDate, int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default)
    {
        return await _sql.QueryAsync<DailyAttendanceRowDto>(SP, new
        {
            ActionType = "DAILY",
            StartDate = startDate,
            EndDate = endDate,
            DepartmentId = departmentId,
            BranchId = branchId,
            EmployeeId = employeeId
        }, ct);
    }

    public async Task<IEnumerable<TardinessRowDto>> GetTardinessAsync(
        DateTime startDate, DateTime endDate, int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default)
    {
        return await _sql.QueryAsync<TardinessRowDto>(SP, new
        {
            ActionType = "TARDINESS",
            StartDate = startDate,
            EndDate = endDate,
            DepartmentId = departmentId,
            BranchId = branchId,
            EmployeeId = employeeId
        }, ct);
    }

    public async Task<IEnumerable<AbsenteeismRowDto>> GetAbsenteeismAsync(
        DateTime startDate, DateTime endDate, int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default)
    {
        return await _sql.QueryAsync<AbsenteeismRowDto>(SP, new
        {
            ActionType = "ABSENTEEISM",
            StartDate = startDate,
            EndDate = endDate,
            DepartmentId = departmentId,
            BranchId = branchId,
            EmployeeId = employeeId
        }, ct);
    }

    public async Task<IEnumerable<OvertimeRowDto>> GetOvertimeAsync(
        DateTime startDate, DateTime endDate, int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default)
    {
        return await _sql.QueryAsync<OvertimeRowDto>(SP, new
        {
            ActionType = "OVERTIME",
            StartDate = startDate,
            EndDate = endDate,
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
            StartDate = (DateTime?)null,
            EndDate = (DateTime?)null,
            DepartmentId = departmentId,
            BranchId = branchId,
            EmployeeId = employeeId
        }, ct);
    }

    public async Task<IEnumerable<AttendanceSummaryRowDto>> GetSummaryAsync(
        DateTime startDate, DateTime endDate, int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default)
    {
        return await _sql.QueryAsync<AttendanceSummaryRowDto>(SP, new
        {
            ActionType = "SUMMARY",
            StartDate = startDate,
            EndDate = endDate,
            DepartmentId = departmentId,
            BranchId = branchId,
            EmployeeId = employeeId
        }, ct);
    }
}
