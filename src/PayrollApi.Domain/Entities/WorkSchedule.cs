namespace PayrollApi.Domain.Entities;

public class WorkSchedule : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsDefault { get; set; }

    // Schedule rules (per-schedule)
    public decimal RegularHoursPerDay { get; set; } = 8.0m;
    public decimal HalfDayThresholdHours { get; set; } = 4.0m;
    public int GracePeriodMinutes { get; set; } = 15;
    public int BreakDurationMinutes { get; set; } = 60;
    public TimeSpan NightDiffStartTime { get; set; } = new(22, 0, 0);
    public TimeSpan NightDiffEndTime { get; set; } = new(6, 0, 0);
    public decimal NightDiffRate { get; set; } = 1.10m;
    public int OTMinimumMinutes { get; set; } = 30;
    public int OTStartAfterMinutes { get; set; }
    public bool OTRequiresApproval { get; set; } = true;
    public bool AllowNightDifferential { get; set; } = true;
    public bool AllowOvertime { get; set; } = true;

    public ICollection<WorkScheduleDay> Days { get; set; } = new List<WorkScheduleDay>();
    public ICollection<EmployeeSchedule> EmployeeSchedules { get; set; } = new List<EmployeeSchedule>();
}
