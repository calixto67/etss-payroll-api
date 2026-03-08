namespace PayrollApi.Application.DTOs.WorkSchedule;

public sealed class WorkScheduleDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsDefault { get; init; }
    public int EmployeeCount { get; init; }
    public List<WorkScheduleDayDto> Days { get; init; } = new();
}

public sealed class CreateWorkScheduleDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsDefault { get; init; }
    public List<WorkScheduleDayInputDto> Days { get; init; } = new();
}

public sealed class UpdateWorkScheduleDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsDefault { get; init; }
    public List<WorkScheduleDayInputDto> Days { get; init; } = new();
}
