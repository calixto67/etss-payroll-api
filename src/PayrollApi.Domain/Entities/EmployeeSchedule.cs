namespace PayrollApi.Domain.Entities;

public class EmployeeSchedule : BaseEntity
{
    public int EmployeeId { get; set; }
    public int WorkScheduleId { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }

    public Employee Employee { get; set; } = null!;
    public WorkSchedule WorkSchedule { get; set; } = null!;
}
