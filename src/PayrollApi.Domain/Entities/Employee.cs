namespace PayrollApi.Domain.Entities;

/// <summary>
/// Core employee domain entity.
/// </summary>
public class Employee : BaseEntity
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public EmploymentStatus Status { get; set; } = EmploymentStatus.Active;
    public EmploymentType EmploymentType { get; set; } = EmploymentType.FullTime;
    public int DepartmentId { get; set; }
    public int PositionId { get; set; }
    public decimal BasicSalary { get; set; }
    public string BankAccountNumber { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string TaxIdentificationNumber { get; set; } = string.Empty;
    public string SssNumber { get; set; } = string.Empty;
    public string PhilHealthNumber { get; set; } = string.Empty;
    public string PagIbigNumber { get; set; } = string.Empty;

    // Navigation
    public Department? Department { get; set; }
    public Position? Position { get; set; }
    public ICollection<PayrollRecord> PayrollRecords { get; set; } = new List<PayrollRecord>();
}

public enum EmploymentStatus { Active = 1, Inactive = 2, OnLeave = 3, Terminated = 4 }
public enum EmploymentType { FullTime = 1, PartTime = 2, Contractual = 3, Probationary = 4 }
