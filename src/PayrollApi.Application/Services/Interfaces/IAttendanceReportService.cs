using PayrollApi.Application.DTOs.Reports;

namespace PayrollApi.Application.Services.Interfaces;

public interface IAttendanceReportService
{
    Task<IEnumerable<DailyAttendanceRowDto>> GetDailyAsync(DateTime startDate, DateTime endDate, int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default);
    Task<IEnumerable<TardinessRowDto>> GetTardinessAsync(DateTime startDate, DateTime endDate, int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default);
    Task<IEnumerable<AbsenteeismRowDto>> GetAbsenteeismAsync(DateTime startDate, DateTime endDate, int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default);
    Task<IEnumerable<OvertimeRowDto>> GetOvertimeAsync(DateTime startDate, DateTime endDate, int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default);
    Task<IEnumerable<LeaveUsageRowDto>> GetLeaveUsageAsync(int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default);
    Task<IEnumerable<AttendanceSummaryRowDto>> GetSummaryAsync(DateTime startDate, DateTime endDate, int? departmentId, int? branchId, int? employeeId, CancellationToken ct = default);
}
