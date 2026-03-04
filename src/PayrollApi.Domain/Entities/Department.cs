namespace PayrollApi.Domain.Entities;

public class Department : BaseEntity
{
    public string DepartmentCode { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ManagerId { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
