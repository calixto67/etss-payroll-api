namespace PayrollApi.Application.DTOs.Employee;

public sealed class UpdateEmployeeDto
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public int DepartmentId { get; init; }
    public int PositionId { get; init; }
    public decimal BasicSalary { get; init; }
    public string BankAccountNumber { get; init; } = string.Empty;
    public string BankName { get; init; } = string.Empty;
    public int Status { get; init; }
    public int EmploymentType { get; init; }
}
