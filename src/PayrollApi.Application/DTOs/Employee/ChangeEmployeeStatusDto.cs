namespace PayrollApi.Application.DTOs.Employee;

public sealed class ChangeEmployeeStatusDto
{
    public int     NewStatus      { get; init; }
    public string  Remarks        { get; init; } = string.Empty;
    public DateTime? LastWorkingDate { get; init; }
}
