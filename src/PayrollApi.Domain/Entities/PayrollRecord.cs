namespace PayrollApi.Domain.Entities;

/// <summary>
/// Represents a single payroll computation run for one employee in one period.
/// </summary>
public class PayrollRecord : BaseEntity
{
    public int EmployeeId { get; set; }
    public int PayrollPeriodId { get; set; }
    public decimal BasicPay { get; set; }
    public decimal OvertimePay { get; set; }
    public decimal HolidayPay { get; set; }
    public decimal Allowances { get; set; }
    public decimal GrossPay { get; set; }

    // Deductions
    public decimal SssDeduction { get; set; }
    public decimal PhilHealthDeduction { get; set; }
    public decimal PagIbigDeduction { get; set; }
    public decimal TaxWithheld { get; set; }
    public decimal OtherDeductions { get; set; }
    public decimal TotalDeductions { get; set; }

    public decimal NetPay { get; set; }
    public PayrollStatus Status { get; set; } = PayrollStatus.Draft;
    public DateTime? ProcessedAt { get; set; }
    public string? ProcessedBy { get; set; }
    public string? Remarks { get; set; }

    // Navigation
    public Employee? Employee { get; set; }
    public PayrollPeriod? PayrollPeriod { get; set; }
}

public enum PayrollStatus { Draft = 1, ForApproval = 2, Approved = 3, Released = 4, Cancelled = 5 }
