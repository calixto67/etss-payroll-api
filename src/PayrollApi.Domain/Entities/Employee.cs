namespace PayrollApi.Domain.Entities;

/// <summary>
/// Core employee domain entity — expanded with full personal, contact, address,
/// employment, compensation, and status-lifecycle fields.
/// </summary>
public class Employee : BaseEntity
{
    // ── Identity ─────────────────────────────────────────────────────────────
    public string EmployeeCode { get; set; } = string.Empty;

    // ── Personal Information ─────────────────────────────────────────────────
    public string  FirstName     { get; set; } = string.Empty;
    public string? MiddleName    { get; set; }
    public string  LastName      { get; set; } = string.Empty;
    public string? Suffix        { get; set; }
    public DateTime DateOfBirth  { get; set; }
    public Gender  Gender        { get; set; } = Gender.Unspecified;
    public MaritalStatus MaritalStatus { get; set; } = MaritalStatus.Single;

    // ── Government IDs ───────────────────────────────────────────────────────
    public string TaxIdentificationNumber { get; set; } = string.Empty;
    public string SssNumber               { get; set; } = string.Empty;
    public string PhilHealthNumber        { get; set; } = string.Empty;
    public string PagIbigNumber           { get; set; } = string.Empty;

    // ── Contact Information ──────────────────────────────────────────────────
    public string  Email         { get; set; } = string.Empty;  // Work email
    public string? PersonalEmail { get; set; }
    public string  MobileNumber  { get; set; } = string.Empty;
    public string? AlternatePhone { get; set; }

    // ── Present Address ──────────────────────────────────────────────────────
    public string PresentAddress  { get; set; } = string.Empty;
    public string PresentCity     { get; set; } = string.Empty;
    public string PresentProvince { get; set; } = string.Empty;
    public string PresentZipCode  { get; set; } = string.Empty;

    // ── Permanent Address ────────────────────────────────────────────────────
    public bool    SameAsPresentAddress { get; set; } = false;
    public string? PermanentAddress     { get; set; }
    public string? PermanentCity        { get; set; }
    public string? PermanentProvince    { get; set; }
    public string? PermanentZipCode     { get; set; }

    // ── Employment Classification ────────────────────────────────────────────
    public int  DepartmentId   { get; set; }
    public int  PositionId     { get; set; }
    public int? ManagerId      { get; set; }   // Self-referencing FK
    public int? BranchId       { get; set; }
    public EmploymentType EmploymentType { get; set; } = EmploymentType.FullTime;

    // ── Employment Dates ─────────────────────────────────────────────────────
    public DateTime  HireDate            { get; set; }
    public DateTime? TerminationDate     { get; set; }
    public DateTime? ProbationEndDate    { get; set; }
    public DateTime? RegularizationDate  { get; set; }
    public DateTime? LastWorkingDate     { get; set; }

    // ── Compensation ─────────────────────────────────────────────────────────
    public decimal         BasicSalary         { get; set; }
    public SalaryFrequency SalaryFrequency      { get; set; } = SalaryFrequency.Monthly;
    public DateTime        SalaryEffectiveDate  { get; set; }
    public string BankAccountNumber { get; set; } = string.Empty;
    public string BankName          { get; set; } = string.Empty;

    // ── Government Mandate Contributions (per-employee, copied from defaults on creation) ──
    /// <summary>SSS employee contribution amount, e.g. 900.</summary>
    public decimal SssContribution { get; set; } = 900m;

    /// <summary>PhilHealth employee contribution amount, e.g. 500.</summary>
    public decimal PhilHealthContribution { get; set; } = 500m;

    /// <summary>Pag-IBIG employee contribution amount, e.g. 200.</summary>
    public decimal PagIbigContribution { get; set; } = 200m;

    // ── Profile ───────────────────────────────────────────────────────────────
    public string? ProfilePhotoPath { get; set; }
    public string? BiometricId      { get; set; }

    // ── Status & Lifecycle ────────────────────────────────────────────────────
    public EmploymentStatus Status          { get; set; } = EmploymentStatus.Active;
    public string?           StatusRemarks  { get; set; }
    public DateTime?         StatusChangedAt { get; set; }
    public string?           StatusChangedBy { get; set; }

    // ── Navigation Properties ─────────────────────────────────────────────────
    public Department?  Department  { get; set; }
    public Position?    Position    { get; set; }
    public Employee?    Manager     { get; set; }
    public Branch?      Branch      { get; set; }

    public ICollection<PayrollRecord>             PayrollRecords   { get; set; } = new List<PayrollRecord>();
    public ICollection<EmployeeStatusHistory>     StatusHistory    { get; set; } = new List<EmployeeStatusHistory>();
    public ICollection<EmployeeEmergencyContact>  EmergencyContacts { get; set; } = new List<EmployeeEmergencyContact>();
    public ICollection<EmployeeDocument>          Documents        { get; set; } = new List<EmployeeDocument>();
    public ICollection<SalaryHistory>              SalaryHistory    { get; set; } = new List<SalaryHistory>();
}

// ── Enumerations ──────────────────────────────────────────────────────────────

public enum EmploymentStatus
{
    Active     = 1,
    Inactive   = 2,
    OnLeave    = 3,
    Terminated = 4,
    Retired    = 5,
    Suspended  = 6
}

public enum EmploymentType
{
    FullTime     = 1,
    PartTime     = 2,
    Contractual  = 3,
    Probationary = 4
}

public enum Gender
{
    Unspecified = 0,
    Male        = 1,
    Female      = 2,
    Other       = 3
}

public enum MaritalStatus
{
    Single   = 0,
    Married  = 1,
    Divorced = 2,
    Widowed  = 3
}

public enum SalaryFrequency
{
    Monthly    = 0,
    SemiMonthly = 1,
    BiWeekly   = 2,
    Weekly     = 3,
    Daily      = 4
}
