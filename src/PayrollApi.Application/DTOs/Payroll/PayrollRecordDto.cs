namespace PayrollApi.Application.DTOs.Payroll;

public sealed class PayrollRecordDto
{
    public int Id { get; init; }
    public int EmployeeId { get; init; }
    public string EmployeeName { get; init; } = string.Empty;
    public string EmployeeCode { get; init; } = string.Empty;
    public int PayrollPeriodId { get; init; }
    public string PeriodCode { get; init; } = string.Empty;
    public string PeriodName { get; init; } = string.Empty;
    public decimal BasicPay { get; init; }
    public decimal OvertimePay { get; init; }
    public decimal HolidayPay { get; init; }
    public decimal Allowances { get; init; }
    public decimal GrossPay { get; init; }
    public decimal SssDeduction { get; init; }
    public decimal PhilHealthDeduction { get; init; }
    public decimal PagIbigDeduction { get; init; }
    public decimal TaxWithheld { get; init; }
    public decimal OtherDeductions { get; init; }
    public decimal TotalDeductions { get; init; }
    public decimal NetPay { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime? ProcessedAt { get; init; }
    public string? ProcessedBy { get; init; }
    public DateTime CreatedAt { get; init; }
}
