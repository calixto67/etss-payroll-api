namespace PayrollApi.Domain.Entities;

public class EmployeeAllowance : BaseEntity
{
    public int EmployeeId { get; set; }
    public int AllowanceTypeId { get; set; }
    public decimal Amount { get; set; }
    public bool IsActive { get; set; } = true;
    public string Frequency { get; set; } = "Monthly";
    public string? Remarks { get; set; }
}
