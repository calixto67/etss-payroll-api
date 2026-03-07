namespace PayrollApi.Application.DTOs.Leave;

public record LeaveApplicationDto(
    int      Id,
    string   RefNo,
    string   Employee,
    string   LeaveType,
    string   StartDate,
    string   EndDate,
    int      Days,
    string   Reason,
    string   Status,
    string   SubmittedOn,
    string   Approver,
    string?  Remarks
);

public record CreateLeaveApplicationDto(
    string Employee,
    string LeaveType,
    string StartDate,
    string EndDate,
    string Reason,
    string Approver
);

public record UpdateLeaveApplicationStatusDto(
    string Status,
    string? Remarks
);
