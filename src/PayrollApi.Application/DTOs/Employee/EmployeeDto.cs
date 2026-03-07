namespace PayrollApi.Application.DTOs.Employee;

public sealed class EmployeeDto
{
    public int    Id           { get; init; }
    public string EmployeeCode { get; init; } = string.Empty;

    // Personal
    public string  FirstName   { get; init; } = string.Empty;
    public string? MiddleName  { get; init; }
    public string  LastName    { get; init; } = string.Empty;
    public string? Suffix      { get; init; }
    public string  FullName    => string.IsNullOrWhiteSpace(MiddleName)
        ? $"{FirstName} {LastName}"
        : $"{FirstName} {MiddleName} {LastName}";
    public DateTime DateOfBirth { get; init; }
    public string  Gender       { get; init; } = string.Empty;
    public string  MaritalStatus { get; init; } = string.Empty;

    // Contact
    public string  Email         { get; init; } = string.Empty;
    public string  MobileNumber  { get; init; } = string.Empty;
    public string? AlternatePhone { get; init; }

    // Employment
    public DateTime  HireDate       { get; init; }
    public DateTime? LastWorkingDate { get; init; }
    public string    Status          { get; init; } = string.Empty;
    public string?   StatusRemarks   { get; init; }
    public string    EmploymentType  { get; init; } = string.Empty;

    public int     DepartmentId   { get; init; }
    public string? DepartmentName { get; init; }
    public int     PositionId     { get; init; }
    public string? PositionTitle  { get; init; }
    public int?    ManagerId      { get; init; }
    public string? ManagerName    { get; init; }
    public int?    BranchId       { get; init; }
    public string? BranchName     { get; init; }

    // Compensation
    public decimal BasicSalary    { get; init; }
    public string  SalaryFrequency { get; init; } = string.Empty;

    // Government IDs
    public string SssNumber { get; init; } = string.Empty;

    // Audit
    public DateTime  CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
