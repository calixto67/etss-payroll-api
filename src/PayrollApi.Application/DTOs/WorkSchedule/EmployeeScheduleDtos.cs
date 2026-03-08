namespace PayrollApi.Application.DTOs.WorkSchedule;

public sealed class EmployeeScheduleDto
{
    public int Id { get; init; }
    public int EmployeeId { get; init; }
    public string EmployeeName { get; init; } = string.Empty;
    public string EmployeeCode { get; init; } = string.Empty;
    public int WorkScheduleId { get; init; }
    public string WorkScheduleName { get; init; } = string.Empty;
    public DateTime EffectiveDate { get; init; }
    public DateTime? EndDate { get; init; }
}

public sealed class AssignEmployeeScheduleDto
{
    public List<int> EmployeeIds { get; init; } = new();
    public DateTime EffectiveDate { get; init; }
}
