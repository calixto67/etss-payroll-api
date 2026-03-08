namespace PayrollApi.Domain.Entities;

public class WorkScheduleDay : BaseEntity
{
    public int WorkScheduleId { get; set; }
    public int DayOfWeek { get; set; } // 0=Sun to 6=Sat
    public bool IsRestDay { get; set; }
    public TimeSpan? ShiftStart { get; set; }
    public TimeSpan? ShiftEnd { get; set; }
    public TimeSpan? BreakStart { get; set; }
    public TimeSpan? BreakEnd { get; set; }

    public WorkSchedule WorkSchedule { get; set; } = null!;
}
