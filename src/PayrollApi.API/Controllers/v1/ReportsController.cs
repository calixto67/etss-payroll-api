using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApi.Application.Common.Models;
using PayrollApi.Application.DTOs.Reports;
using PayrollApi.Application.Services.Interfaces;

namespace PayrollApi.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/reports")]
[Authorize(Policy = "PayrollViewer")]
public class ReportsController : BaseController
{
    private readonly IPayrollSummaryReportService _payrollSummary;
    private readonly IGovernmentReportService _govReport;
    private readonly IAttendanceReportService _attendanceReport;

    public ReportsController(
        IPayrollSummaryReportService payrollSummary,
        IGovernmentReportService govReport,
        IAttendanceReportService attendanceReport)
    {
        _payrollSummary = payrollSummary;
        _govReport = govReport;
        _attendanceReport = attendanceReport;
    }

    [HttpGet("payroll-summary")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PayrollSummaryRowDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPayrollSummary(
        [FromQuery] int periodId,
        [FromQuery] int? departmentId,
        CancellationToken ct)
    {
        var rows = await _payrollSummary.GetByPeriodAsync(periodId, departmentId, ct);
        return Ok(ApiResponse<IEnumerable<PayrollSummaryRowDto>>.Ok(rows));
    }

    // ── Government Reports ──────────────────────────────────────────

    [HttpGet("government/sss")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SssReportRowDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSssReport(
        [FromQuery] int periodId, [FromQuery] int? departmentId,
        [FromQuery] int? branchId, [FromQuery] int? employeeId, CancellationToken ct)
    {
        var rows = await _govReport.GetSssReportAsync(periodId, departmentId, branchId, employeeId, ct);
        return Ok(ApiResponse<IEnumerable<SssReportRowDto>>.Ok(rows));
    }

    [HttpGet("government/philhealth")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PhilHealthReportRowDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPhilHealthReport(
        [FromQuery] int periodId, [FromQuery] int? departmentId,
        [FromQuery] int? branchId, [FromQuery] int? employeeId, CancellationToken ct)
    {
        var rows = await _govReport.GetPhilHealthReportAsync(periodId, departmentId, branchId, employeeId, ct);
        return Ok(ApiResponse<IEnumerable<PhilHealthReportRowDto>>.Ok(rows));
    }

    [HttpGet("government/pag-ibig")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PagIbigReportRowDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagIbigReport(
        [FromQuery] int periodId, [FromQuery] int? departmentId,
        [FromQuery] int? branchId, [FromQuery] int? employeeId, CancellationToken ct)
    {
        var rows = await _govReport.GetPagIbigReportAsync(periodId, departmentId, branchId, employeeId, ct);
        return Ok(ApiResponse<IEnumerable<PagIbigReportRowDto>>.Ok(rows));
    }

    [HttpGet("government/bir")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BirReportRowDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBirReport(
        [FromQuery] int periodId, [FromQuery] int? departmentId,
        [FromQuery] int? branchId, [FromQuery] int? employeeId, CancellationToken ct)
    {
        var rows = await _govReport.GetBirReportAsync(periodId, departmentId, branchId, employeeId, ct);
        return Ok(ApiResponse<IEnumerable<BirReportRowDto>>.Ok(rows));
    }

    // ── Attendance Reports ────────────────────────────────────────────

    [HttpGet("attendance/daily")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<DailyAttendanceRowDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDailyAttendance(
        [FromQuery] DateTime startDate, [FromQuery] DateTime endDate,
        [FromQuery] int? departmentId, [FromQuery] int? branchId,
        [FromQuery] int? employeeId, CancellationToken ct)
    {
        var rows = await _attendanceReport.GetDailyAsync(startDate, endDate, departmentId, branchId, employeeId, ct);
        return Ok(ApiResponse<IEnumerable<DailyAttendanceRowDto>>.Ok(rows));
    }

    [HttpGet("attendance/tardiness")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TardinessRowDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTardiness(
        [FromQuery] DateTime startDate, [FromQuery] DateTime endDate,
        [FromQuery] int? departmentId, [FromQuery] int? branchId,
        [FromQuery] int? employeeId, CancellationToken ct)
    {
        var rows = await _attendanceReport.GetTardinessAsync(startDate, endDate, departmentId, branchId, employeeId, ct);
        return Ok(ApiResponse<IEnumerable<TardinessRowDto>>.Ok(rows));
    }

    [HttpGet("attendance/absenteeism")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<AbsenteeismRowDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAbsenteeism(
        [FromQuery] DateTime startDate, [FromQuery] DateTime endDate,
        [FromQuery] int? departmentId, [FromQuery] int? branchId,
        [FromQuery] int? employeeId, CancellationToken ct)
    {
        var rows = await _attendanceReport.GetAbsenteeismAsync(startDate, endDate, departmentId, branchId, employeeId, ct);
        return Ok(ApiResponse<IEnumerable<AbsenteeismRowDto>>.Ok(rows));
    }

    [HttpGet("attendance/overtime")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<OvertimeRowDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOvertime(
        [FromQuery] DateTime startDate, [FromQuery] DateTime endDate,
        [FromQuery] int? departmentId, [FromQuery] int? branchId,
        [FromQuery] int? employeeId, CancellationToken ct)
    {
        var rows = await _attendanceReport.GetOvertimeAsync(startDate, endDate, departmentId, branchId, employeeId, ct);
        return Ok(ApiResponse<IEnumerable<OvertimeRowDto>>.Ok(rows));
    }

    [HttpGet("attendance/leave-usage")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<LeaveUsageRowDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLeaveUsage(
        [FromQuery] int? departmentId, [FromQuery] int? branchId,
        [FromQuery] int? employeeId, CancellationToken ct)
    {
        var rows = await _attendanceReport.GetLeaveUsageAsync(departmentId, branchId, employeeId, ct);
        return Ok(ApiResponse<IEnumerable<LeaveUsageRowDto>>.Ok(rows));
    }

    [HttpGet("attendance/summary")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<AttendanceSummaryRowDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAttendanceSummary(
        [FromQuery] DateTime startDate, [FromQuery] DateTime endDate,
        [FromQuery] int? departmentId, [FromQuery] int? branchId,
        [FromQuery] int? employeeId, CancellationToken ct)
    {
        var rows = await _attendanceReport.GetSummaryAsync(startDate, endDate, departmentId, branchId, employeeId, ct);
        return Ok(ApiResponse<IEnumerable<AttendanceSummaryRowDto>>.Ok(rows));
    }
}
