namespace PayrollApi.Application.DTOs.Attendance;

public record AttendanceDto(
    int Id,
    int PayrollPeriodId,
    int EmployeeId,
    string EmployeeCode,
    string EmployeeName,
    decimal DaysWorked,
    decimal TotalDays,
    decimal LateHours,
    decimal UndertimeHours,
    decimal OtHours,
    decimal NightDiffHours,
    string Status,
    string? Issue,
    string? ResolutionNotes,
    List<AttendanceDetailDto>? Details);

public record AttendanceDetailDto(
    int Id,
    int AttendanceId,
    DateTime Date,
    string? TimeIn,
    string? TimeOut,
    decimal LateHours,
    decimal UndertimeHours,
    decimal OtHours,
    decimal NightDiffHours,
    string Status,
    string? Remarks);

public record CreateAttendanceDto(
    int? EmployeeId,
    string? EmployeeCode,
    decimal DaysWorked,
    decimal TotalDays,
    decimal LateHours,
    decimal UndertimeHours,
    decimal OtHours,
    decimal NightDiffHours,
    string Status,
    string? Issue);

public record UpdateAttendanceDto(
    decimal DaysWorked,
    decimal TotalDays,
    decimal LateHours,
    decimal UndertimeHours,
    decimal OtHours,
    decimal NightDiffHours,
    string Status,
    string? Issue);

public record UpdateAttendanceDetailDto(
    int Id,
    DateTime? Date,
    string? TimeIn,
    string? TimeOut,
    decimal LateHours,
    decimal UndertimeHours,
    decimal OtHours,
    decimal NightDiffHours,
    string Status,
    string? Remarks);

public record ResolveAttendanceDto(
    string Resolution,
    string? Notes,
    decimal? DaysWorked,
    decimal? LateHours,
    decimal? UndertimeHours,
    decimal? OtHours,
    decimal? NightDiffHours);

public record ImportAttendanceRowDto(
    string EmployeeCode,
    decimal DaysWorked,
    decimal TotalDays,
    decimal LateHours,
    decimal UndertimeHours,
    decimal OtHours,
    decimal NightDiffHours,
    List<ImportAttendanceDetailDto>? Details);

public record ImportAttendanceDetailDto(
    DateTime Date,
    string? TimeIn,
    string? TimeOut,
    decimal LateHours,
    decimal UndertimeHours,
    decimal OtHours,
    decimal NightDiffHours,
    string Status,
    string? Remarks);

// Raw punch import — let backend compute late/UT/OT/ND from work schedules
public record ImportRawPunchDto(
    string EmployeeCode,
    string Timestamp,
    int PunchType); // 0=In, 1=Out, 2=BreakOut, 3=BreakIn

// Schedule check result
public record ScheduleCheckResultDto(
    string EmployeeCode,
    string EmployeeName,
    bool HasSchedule);
