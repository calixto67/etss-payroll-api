namespace PayrollApi.Domain.Entities;

public class Position : BaseEntity
{
    public string PositionCode { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public decimal MinSalary { get; set; }
    public decimal MaxSalary { get; set; }

    public Department? Department { get; set; }
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
