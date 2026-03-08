namespace PayrollApi.Domain.Entities;

public class ScheduleRule : BaseEntity
{
    public decimal HalfDayThresholdHours { get; set; } = 4.0m;
    public TimeSpan NightDiffStartTime { get; set; } = new(22, 0, 0);
    public TimeSpan NightDiffEndTime { get; set; } = new(6, 0, 0);
    public decimal NightDiffRate { get; set; } = 1.10m;
    public int OTMinimumMinutes { get; set; } = 30;
    public bool OTRequiresApproval { get; set; } = true;
    public int OTStartAfterMinutes { get; set; }
    public int GracePeriodMinutes { get; set; } = 15;
    public int BreakDurationMinutes { get; set; } = 60;
    public decimal RegularHoursPerDay { get; set; } = 8.0m;
}
