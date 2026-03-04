namespace PayrollApi.Application.DTOs.Employee;

public sealed class EmployeeDto
{
    public int Id { get; init; }
    public string EmployeeCode { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public DateTime DateOfBirth { get; init; }
    public DateTime HireDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public string EmploymentType { get; init; } = string.Empty;
    public int DepartmentId { get; init; }
    public string? DepartmentName { get; init; }
    public int PositionId { get; init; }
    public string? PositionTitle { get; init; }
    public decimal BasicSalary { get; init; }
    // Sensitive fields intentionally masked in list responses
    public string BankAccountNumberMasked => MaskAccountNumber(string.Empty);
    public string SssNumber { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }

    private static string MaskAccountNumber(string acct) =>
        acct.Length > 4 ? $"****{acct[^4..]}" : "****";
}
