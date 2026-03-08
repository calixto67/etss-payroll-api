using PayrollApi.Application.DTOs.Reports;

namespace PayrollApi.Application.Services.Interfaces;

public interface IAttendanceReportService
{
    Task<IEnumerable<DailyAttendanceRowDto>> GetDailyAsync(int periodId, int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default);
    Task<IEnumerable<TardinessRowDto>> GetTardinessAsync(int periodId, int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default);
    Task<IEnumerable<AbsenteeismRowDto>> GetAbsenteeismAsync(int periodId, int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default);
    Task<IEnumerable<OvertimeRowDto>> GetOvertimeAsync(int periodId, int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default);
    Task<IEnumerable<LeaveUsageRowDto>> GetLeaveUsageAsync(int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default);
    Task<IEnumerable<AttendanceSummaryRowDto>> GetSummaryAsync(int periodId, int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default);
}
