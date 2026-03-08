namespace PayrollApi.Application.DTOs.OvertimeApplication;

public record OvertimeApplicationDto(
    int      Id,
    string   RefNo,
    string   Employee,
    string   EmployeeCode,
    int?     PayrollPeriodId,
    string   OvertimeDate,
    string   StartTime,
    string   EndTime,
    decimal  TotalHours,
    string   Reason,
    string   Status,
    string?  Approver,
    string?  Remarks,
    string   SubmittedOn
);

public record CreateOvertimeApplicationDto(
    string  Employee,
    string  EmployeeCode,
    int?    PayrollPeriodId,
    string  OvertimeDate,
    string  StartTime,
    string  EndTime,
    decimal? TotalHours,
    string  Reason,
    string? Approver
);

public record UpdateOvertimeApplicationStatusDto(
    string  Status,
    string? Remarks
);
