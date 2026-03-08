namespace PayrollApi.Application.DTOs.WorkSchedule;

public sealed class WorkScheduleDayDto
{
    public int Id { get; init; }
    public int DayOfWeek { get; init; }
    public bool IsRestDay { get; init; }
    public string? ShiftStart { get; init; }
    public string? ShiftEnd { get; init; }
    public string? BreakStart { get; init; }
    public string? BreakEnd { get; init; }
}

public sealed class WorkScheduleDayInputDto
{
    public int DayOfWeek { get; init; }
    public bool IsRestDay { get; init; }
    public string? ShiftStart { get; init; }
    public string? ShiftEnd { get; init; }
    public string? BreakStart { get; init; }
    public string? BreakEnd { get; init; }
}
