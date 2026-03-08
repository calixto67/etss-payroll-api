namespace PayrollApi.Application.DTOs.WorkSchedule;

public sealed class ScheduleRuleDto
{
    public int Id { get; init; }
    public decimal HalfDayThresholdHours { get; init; }
    public string NightDiffStartTime { get; init; } = string.Empty;
    public string NightDiffEndTime { get; init; } = string.Empty;
    public decimal NightDiffRate { get; init; }
    public int OTMinimumMinutes { get; init; }
    public bool OTRequiresApproval { get; init; }
    public int OTStartAfterMinutes { get; init; }
    public int GracePeriodMinutes { get; init; }
    public int BreakDurationMinutes { get; init; }
    public decimal RegularHoursPerDay { get; init; }
}

public sealed class UpdateScheduleRuleDto
{
    public decimal HalfDayThresholdHours { get; init; }
    public string NightDiffStartTime { get; init; } = string.Empty;
    public string NightDiffEndTime { get; init; } = string.Empty;
    public decimal NightDiffRate { get; init; }
    public int OTMinimumMinutes { get; init; }
    public bool OTRequiresApproval { get; init; }
    public int OTStartAfterMinutes { get; init; }
    public int GracePeriodMinutes { get; init; }
    public int BreakDurationMinutes { get; init; }
    public decimal RegularHoursPerDay { get; init; }
}
