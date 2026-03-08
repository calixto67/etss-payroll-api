namespace PayrollApi.Application.DTOs.Employee;

public sealed class CreateEmployeeDto
{
    // Identity
    public string EmployeeCode { get; init; } = string.Empty;

    // Personal
    public string  FirstName    { get; init; } = string.Empty;
    public string? MiddleName   { get; init; }
    public string  LastName     { get; init; } = string.Empty;
    public string? Suffix       { get; init; }
    public DateTime DateOfBirth { get; init; }
    public int     Gender       { get; init; } = 0;
    public int     MaritalStatus { get; init; } = 0;

    // Government IDs
    public string TaxIdentificationNumber { get; init; } = string.Empty;
    public string SssNumber               { get; init; } = string.Empty;
    public string PhilHealthNumber        { get; init; } = string.Empty;
    public string PagIbigNumber           { get; init; } = string.Empty;

    // Contact
    public string  Email          { get; init; } = string.Empty;
    public string? PersonalEmail  { get; init; }
    public string  MobileNumber   { get; init; } = string.Empty;
    public string? AlternatePhone { get; init; }

    // Present Address
    public string PresentAddress  { get; init; } = string.Empty;
    public string PresentCity     { get; init; } = string.Empty;
    public string PresentProvince { get; init; } = string.Empty;
    public string PresentZipCode  { get; init; } = string.Empty;

    // Permanent Address
    public bool    SameAsPresentAddress { get; init; } = false;
    public string? PermanentAddress     { get; init; }
    public string? PermanentCity        { get; init; }
    public string? PermanentProvince    { get; init; }
    public string? PermanentZipCode     { get; init; }

    // Employment
    public int  DepartmentId   { get; init; }
    public int  PositionId     { get; init; }
    public int? ManagerId      { get; init; }
    public int? BranchId       { get; init; }
    public int  EmploymentType { get; init; } = 1;

    // Dates
    public DateTime  HireDate          { get; init; }
    public DateTime? ProbationEndDate  { get; init; }

    // Compensation
    public decimal  BasicSalary         { get; init; }
    public int      SalaryFrequency     { get; init; } = 0;
    public DateTime SalaryEffectiveDate { get; init; }
    public string   BankAccountNumber   { get; init; } = string.Empty;
    public string   BankName            { get; init; } = string.Empty;

    // Other
    public string? BiometricId { get; init; }
}
