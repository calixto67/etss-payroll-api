namespace PayrollApi.Domain.Entities;

public class Branch : BaseEntity
{
    public string  BranchCode    { get; set; } = string.Empty;
    public string  BranchName    { get; set; } = string.Empty;
    public string? Address       { get; set; }
    public bool    IsHeadOffice  { get; set; } = false;

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
