namespace PayrollApi.Application.DTOs.Reports;

// ── Daily Attendance ────────────────────────────────────────────
public sealed class DailyAttendanceRowDto
{
    public int EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = "";
    public string EmployeeName { get; set; } = "";
    public string? DepartmentName { get; set; }
    public string? BranchName { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan? TimeIn { get; set; }
    public TimeSpan? TimeOut { get; set; }
    public decimal LateHours { get; set; }
    public decimal UndertimeHours { get; set; }
    public decimal OtHours { get; set; }
    public decimal NightDiffHours { get; set; }
    public string? Status { get; set; }
    public string? Remarks { get; set; }
}

// ── Tardiness & Undertime ───────────────────────────────────────
public sealed class TardinessRowDto
{
    public int EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = "";
    public string EmployeeName { get; set; } = "";
    public string? DepartmentName { get; set; }
    public string? BranchName { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan? TimeIn { get; set; }
    public TimeSpan? TimeOut { get; set; }
    public decimal LateHours { get; set; }
    public decimal UndertimeHours { get; set; }
}

// ── Absenteeism ─────────────────────────────────────────────────
public sealed class AbsenteeismRowDto
{
    public int EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = "";
    public string EmployeeName { get; set; } = "";
    public string? DepartmentName { get; set; }
    public string? BranchName { get; set; }
    public DateTime Date { get; set; }
    public string? Status { get; set; }
    public string? Remarks { get; set; }
}

// ── Overtime ────────────────────────────────────────────────────
public sealed class OvertimeRowDto
{
    public int EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = "";
    public string EmployeeName { get; set; } = "";
    public string? DepartmentName { get; set; }
    public string? BranchName { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan? TimeIn { get; set; }
    public TimeSpan? TimeOut { get; set; }
    public decimal OtHours { get; set; }
    public decimal NightDiffHours { get; set; }
    public string? Status { get; set; }
}

// ── Leave Usage ─────────────────────────────────────────────────
public sealed class LeaveUsageRowDto
{
    public int EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = "";
    public string EmployeeName { get; set; } = "";
    public string? DepartmentName { get; set; }
    public string? BranchName { get; set; }
    public string LeaveType { get; set; } = "";
    public int Entitlement { get; set; }
    public int Used { get; set; }
    public int Pending { get; set; }
    public int CarryOver { get; set; }
    public int Remaining { get; set; }
}

// ── Attendance Summary ──────────────────────────────────────────
public sealed class AttendanceSummaryRowDto
{
    public int EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = "";
    public string EmployeeName { get; set; } = "";
    public string? DepartmentName { get; set; }
    public string? BranchName { get; set; }
    public decimal DaysWorked { get; set; }
    public decimal TotalDays { get; set; }
    public decimal TotalLateHours { get; set; }
    public decimal TotalUndertimeHours { get; set; }
    public decimal TotalOtHours { get; set; }
    public decimal TotalNightDiffHours { get; set; }
    public decimal DaysAbsent { get; set; }
    public decimal AttendanceRate { get; set; }
    public string? Status { get; set; }
}
