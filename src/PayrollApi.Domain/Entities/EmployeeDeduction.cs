namespace PayrollApi.Domain.Entities;

public class EmployeeDeduction : BaseEntity
{
    public int EmployeeId { get; set; }
    public int DeductionTypeId { get; set; }
    public decimal Amount { get; set; }
    public bool IsActive { get; set; } = true;
    public string Frequency { get; set; } = "Monthly";
    public string? Remarks { get; set; }
}
