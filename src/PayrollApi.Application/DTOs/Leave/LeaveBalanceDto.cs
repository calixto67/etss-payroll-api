namespace PayrollApi.Application.DTOs.Leave;

public record LeaveBalanceDto(
    int    Id,
    string EmployeeId,
    string EmployeeName,
    string LeaveType,
    int    Entitlement,
    int    Used,
    int    Pending,
    int    CarryOver,
    int    Remaining
);

public record CreateLeaveBalanceDto(
    string EmployeeCode,
    string EmployeeName,
    string LeaveType,
    int    Entitlement,
    int    CarryOver = 0
);

public record UpdateLeaveBalanceDto(
    int Entitlement,
    int Used,
    int Pending,
    int CarryOver
);

public record EnrollAllDto(
    string LeaveType,
    int    Entitlement
);
