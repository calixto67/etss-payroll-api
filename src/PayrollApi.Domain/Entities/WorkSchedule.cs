namespace PayrollApi.Domain.Entities;

public class WorkSchedule : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsDefault { get; set; }

    public ICollection<WorkScheduleDay> Days { get; set; } = new List<WorkScheduleDay>();
    public ICollection<EmployeeSchedule> EmployeeSchedules { get; set; } = new List<EmployeeSchedule>();
}
