namespace PayrollApi.Application.DTOs.WorkSchedule;

public sealed class WorkScheduleDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsDefault { get; init; }
    public int EmployeeCount { get; init; }
    public List<WorkScheduleDayDto> Days { get; init; } = new();

    // Schedule rules
    public decimal RegularHoursPerDay { get; init; }
    public decimal HalfDayThresholdHours { get; init; }
    public int GracePeriodMinutes { get; init; }
    public int BreakDurationMinutes { get; init; }
    public string? NightDiffStartTime { get; init; }
    public string? NightDiffEndTime { get; init; }
    public decimal NightDiffRate { get; init; }
    public int OTMinimumMinutes { get; init; }
    public int OTStartAfterMinutes { get; init; }
    public bool OTRequiresApproval { get; init; }
    public bool AllowNightDifferential { get; init; }
    public bool AllowOvertime { get; init; }
}

public sealed class CreateWorkScheduleDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsDefault { get; init; }
    public List<WorkScheduleDayInputDto> Days { get; init; } = new();

    // Schedule rules
    public decimal RegularHoursPerDay { get; init; } = 8.0m;
    public decimal HalfDayThresholdHours { get; init; } = 4.0m;
    public int GracePeriodMinutes { get; init; } = 15;
    public int BreakDurationMinutes { get; init; } = 60;
    public string NightDiffStartTime { get; init; } = "22:00";
    public string NightDiffEndTime { get; init; } = "06:00";
    public decimal NightDiffRate { get; init; } = 1.10m;
    public int OTMinimumMinutes { get; init; } = 30;
    public int OTStartAfterMinutes { get; init; }
    public bool OTRequiresApproval { get; init; } = true;
    public bool AllowNightDifferential { get; init; } = true;
    public bool AllowOvertime { get; init; } = true;
}

public sealed class UpdateWorkScheduleDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsDefault { get; init; }
    public List<WorkScheduleDayInputDto> Days { get; init; } = new();

    // Schedule rules
    public decimal RegularHoursPerDay { get; init; } = 8.0m;
    public decimal HalfDayThresholdHours { get; init; } = 4.0m;
    public int GracePeriodMinutes { get; init; } = 15;
    public int BreakDurationMinutes { get; init; } = 60;
    public string NightDiffStartTime { get; init; } = "22:00";
    public string NightDiffEndTime { get; init; } = "06:00";
    public decimal NightDiffRate { get; init; } = 1.10m;
    public int OTMinimumMinutes { get; init; } = 30;
    public int OTStartAfterMinutes { get; init; }
    public bool OTRequiresApproval { get; init; } = true;
    public bool AllowNightDifferential { get; init; } = true;
    public bool AllowOvertime { get; init; } = true;
}
