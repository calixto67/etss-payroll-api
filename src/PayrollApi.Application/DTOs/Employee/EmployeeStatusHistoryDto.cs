namespace PayrollApi.Application.DTOs.Employee;

public sealed class EmployeeStatusHistoryDto
{
    public int      Id             { get; init; }
    public string   PreviousStatus { get; init; } = string.Empty;
    public string   NewStatus      { get; init; } = string.Empty;
    public string   Remarks        { get; init; } = string.Empty;
    public string   ChangedBy      { get; init; } = string.Empty;
    public DateTime ChangedAt      { get; init; }
}
