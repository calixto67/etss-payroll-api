namespace PayrollApi.Domain.Entities;

public class SalaryHistory : BaseEntity
{
    public int      EmployeeId      { get; set; }
    public decimal  PreviousSalary  { get; set; }
    public decimal  NewSalary       { get; set; }
    public int      SalaryFrequency { get; set; }
    public DateTime EffectiveDate   { get; set; }
    public string   ChangedBy       { get; set; } = string.Empty;
    public DateTime ChangedAt       { get; set; }
    public string?  Remarks         { get; set; }

    public Employee? Employee { get; set; }
}
