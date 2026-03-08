namespace PayrollApi.Application.DTOs.Employee;

/// <summary>Full employee detail including all fields and related data.</summary>
public sealed class EmployeeDetailDto
{
    public int    Id           { get; init; }
    public string EmployeeCode { get; init; } = string.Empty;

    // Personal
    public string  FirstName    { get; init; } = string.Empty;
    public string? MiddleName   { get; init; }
    public string  LastName     { get; init; } = string.Empty;
    public string? Suffix       { get; init; }
    public string  FullName     => string.IsNullOrWhiteSpace(MiddleName)
        ? $"{FirstName} {LastName}"
        : $"{FirstName} {MiddleName} {LastName}";
    public DateTime DateOfBirth  { get; init; }
    public string   Gender       { get; init; } = string.Empty;
    public string   MaritalStatus { get; init; } = string.Empty;

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
    public bool    SameAsPresentAddress { get; init; }
    public string? PermanentAddress     { get; init; }
    public string? PermanentCity        { get; init; }
    public string? PermanentProvince    { get; init; }
    public string? PermanentZipCode     { get; init; }

    // Employment
    public int     DepartmentId   { get; init; }
    public string? DepartmentName { get; init; }
    public int     PositionId     { get; init; }
    public string? PositionTitle  { get; init; }
    public int?    ManagerId      { get; init; }
    public string? ManagerName    { get; init; }
    public int?    BranchId       { get; init; }
    public string? BranchName     { get; init; }
    public string  EmploymentType { get; init; } = string.Empty;

    // Dates
    public DateTime  HireDate           { get; init; }
    public DateTime? TerminationDate    { get; init; }
    public DateTime? ProbationEndDate   { get; init; }
    public DateTime? RegularizationDate { get; init; }
    public DateTime? LastWorkingDate    { get; init; }

    // Compensation
    public decimal  BasicSalary         { get; init; }
    public string   SalaryFrequency     { get; init; } = string.Empty;
    public DateTime SalaryEffectiveDate { get; init; }
    public string   BankName            { get; init; } = string.Empty;

    // Profile
    public string? ProfilePhotoPath { get; init; }
    public string? BiometricId      { get; init; }

    // Status
    public string    Status           { get; init; } = string.Empty;
    public string?   StatusRemarks    { get; init; }
    public DateTime? StatusChangedAt  { get; init; }
    public string?   StatusChangedBy  { get; init; }

    // Audit
    public string    CreatedBy { get; init; } = string.Empty;
    public DateTime  CreatedAt { get; init; }
    public string?   UpdatedBy { get; init; }
    public DateTime? UpdatedAt { get; init; }

    // Related data
    public IEnumerable<EmployeeStatusHistoryDto>    StatusHistory     { get; init; } = Enumerable.Empty<EmployeeStatusHistoryDto>();
    public IEnumerable<EmergencyContactDto>         EmergencyContacts { get; init; } = Enumerable.Empty<EmergencyContactDto>();
    public IEnumerable<EmployeeDocumentDto>         Documents         { get; init; } = Enumerable.Empty<EmployeeDocumentDto>();
    public IEnumerable<SalaryHistoryDto>            SalaryHistory     { get; init; } = Enumerable.Empty<SalaryHistoryDto>();
}

public sealed class SalaryHistoryDto
{
    public int      Id              { get; init; }
    public decimal  PreviousSalary  { get; init; }
    public decimal  NewSalary       { get; init; }
    public string   SalaryFrequency { get; init; } = string.Empty;
    public DateTime EffectiveDate   { get; init; }
    public string   ChangedBy       { get; init; } = string.Empty;
    public DateTime ChangedAt       { get; init; }
    public string?  Remarks         { get; init; }
}
