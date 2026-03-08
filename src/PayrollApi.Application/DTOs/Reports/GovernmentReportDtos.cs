namespace PayrollApi.Application.DTOs.Reports;

// ── SSS ────────────────────────────────────────────────────────────
public sealed class SssReportRowDto
{
    public int EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = "";
    public string EmployeeName { get; set; } = "";
    public string? SssNumber { get; set; }
    public string? DepartmentName { get; set; }
    public decimal BasicPay { get; set; }
    public decimal EmployeeShare { get; set; }
    public decimal EmployerShare { get; set; }
    public decimal TotalContribution { get; set; }
    public string? PeriodName { get; set; }
    public DateTime? PeriodStart { get; set; }
    public DateTime? PeriodEnd { get; set; }
    public int Status { get; set; }
}

// ── PhilHealth ─────────────────────────────────────────────────────
public sealed class PhilHealthReportRowDto
{
    public int EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = "";
    public string EmployeeName { get; set; } = "";
    public string? PhilHealthNumber { get; set; }
    public string? DepartmentName { get; set; }
    public decimal BasicPay { get; set; }
    public decimal EmployeeShare { get; set; }
    public decimal EmployerShare { get; set; }
    public decimal TotalPremium { get; set; }
    public string? PeriodName { get; set; }
    public DateTime? PeriodStart { get; set; }
    public DateTime? PeriodEnd { get; set; }
    public int Status { get; set; }
}

// ── Pag-IBIG ───────────────────────────────────────────────────────
public sealed class PagIbigReportRowDto
{
    public int EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = "";
    public string EmployeeName { get; set; } = "";
    public string? PagIbigNumber { get; set; }
    public string? DepartmentName { get; set; }
    public decimal BasicPay { get; set; }
    public decimal EmployeeShare { get; set; }
    public decimal EmployerShare { get; set; }
    public decimal TotalContribution { get; set; }
    public string? PeriodName { get; set; }
    public DateTime? PeriodStart { get; set; }
    public DateTime? PeriodEnd { get; set; }
    public int Status { get; set; }
}

// ── BIR ────────────────────────────────────────────────────────────
public sealed class BirReportRowDto
{
    public int EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = "";
    public string EmployeeName { get; set; } = "";
    public string? Tin { get; set; }
    public string? DepartmentName { get; set; }
    public decimal BasicPay { get; set; }
    public decimal OvertimePay { get; set; }
    public decimal HolidayPay { get; set; }
    public decimal Allowances { get; set; }
    public decimal GrossPay { get; set; }
    public decimal TotalMandatoryDeductions { get; set; }
    public decimal TaxableIncome { get; set; }
    public decimal TaxWithheld { get; set; }
    public decimal NetPay { get; set; }
    public string? PeriodName { get; set; }
    public DateTime? PeriodStart { get; set; }
    public DateTime? PeriodEnd { get; set; }
    public int Status { get; set; }
}
