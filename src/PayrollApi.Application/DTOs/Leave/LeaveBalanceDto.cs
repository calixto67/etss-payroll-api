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
