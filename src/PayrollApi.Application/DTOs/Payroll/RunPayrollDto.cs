namespace PayrollApi.Application.DTOs.Payroll;

public sealed record RunPayrollDto
{
    public int PayrollPeriodId { get; init; }
    public List<int>? EmployeeIds { get; init; } // null = run for all active employees
    public string InitiatedBy { get; init; } = string.Empty;
}
