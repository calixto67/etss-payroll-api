namespace PayrollApi.Application.DTOs.Department;

public sealed class DepartmentDto
{
    public int Id { get; init; }
    public string DepartmentCode { get; init; } = string.Empty;
    public string DepartmentName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int? ManagerId { get; init; }
}

public sealed class CreateDepartmentDto
{
    public string DepartmentCode { get; init; } = string.Empty;
    public string DepartmentName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int? ManagerId { get; init; }
}

public sealed class UpdateDepartmentDto
{
    public string DepartmentCode { get; init; } = string.Empty;
    public string DepartmentName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int? ManagerId { get; init; }
}
