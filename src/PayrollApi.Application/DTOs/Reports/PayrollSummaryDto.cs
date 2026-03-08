namespace PayrollApi.Application.DTOs.Reports;

public class PayrollSummaryRowDto
{
    public int Id { get; set; }
    public string EmployeeCode { get; set; } = "";
    public string EmployeeName { get; set; } = "";
    public string DepartmentName { get; set; } = "";
    public string PositionName { get; set; } = "";
    public decimal BasicPay { get; set; }
    public decimal OvertimePay { get; set; }
    public decimal HolidayPay { get; set; }
    public decimal Allowances { get; set; }
    public decimal GrossPay { get; set; }
    public decimal SssDeduction { get; set; }
    public decimal PhilHealthDeduction { get; set; }
    public decimal PagIbigDeduction { get; set; }
    public decimal TaxWithheld { get; set; }
    public decimal OtherDeductions { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetPay { get; set; }
    public string Status { get; set; } = "";
}
