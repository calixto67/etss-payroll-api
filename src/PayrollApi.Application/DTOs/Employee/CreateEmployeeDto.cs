namespace PayrollApi.Application.DTOs.Employee;

public sealed class CreateEmployeeDto
{
    public string EmployeeCode { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public DateTime DateOfBirth { get; init; }
    public DateTime HireDate { get; init; }
    public int DepartmentId { get; init; }
    public int PositionId { get; init; }
    public decimal BasicSalary { get; init; }
    public string BankAccountNumber { get; init; } = string.Empty;
    public string BankName { get; init; } = string.Empty;
    public string TaxIdentificationNumber { get; init; } = string.Empty;
    public string SssNumber { get; init; } = string.Empty;
    public string PhilHealthNumber { get; init; } = string.Empty;
    public string PagIbigNumber { get; init; } = string.Empty;
    public int EmploymentType { get; init; } = 1;
}
