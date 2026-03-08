namespace PayrollApi.Application.DTOs.Leave;

public record LeaveYearEndResultDto(
    int Year,
    int EmployeesProcessed,
    int BalancesCreated,
    int BalancesExpired,
    int CarryForwardsApplied,
    string Message
);

public record LeaveYearEndBatchDto(
    int Id,
    int Year,
    string ProcessedAt,
    string ProcessedBy,
    int EmployeesProcessed,
    int BalancesCreated,
    int BalancesExpired,
    int CarryForwardsApplied,
    string Status
);
