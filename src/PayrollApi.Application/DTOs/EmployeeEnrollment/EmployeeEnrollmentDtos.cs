namespace PayrollApi.Application.DTOs.EmployeeEnrollment;

public sealed class EmployeeAllowanceDto
{
    public int Id { get; init; }
    public int EmployeeId { get; init; }
    public int AllowanceTypeId { get; init; }
    public decimal Amount { get; init; }
    public bool IsActive { get; init; }
    public string Frequency { get; init; } = "Monthly";
    public string? Remarks { get; init; }
    public string EmployeeCode { get; init; } = string.Empty;
    public string EmployeeName { get; init; } = string.Empty;
    public string AllowanceTypeName { get; init; } = string.Empty;
    public bool IsDeMinimis { get; init; }
    public decimal TaxExemptLimit { get; init; }
    public int? DepartmentId { get; init; }
    public string? DepartmentName { get; init; }
    public int? BranchId { get; init; }
    public string? BranchName { get; init; }
    public string? PositionTitle { get; init; }
}

public sealed class EmployeeDeductionDto
{
    public int Id { get; init; }
    public int EmployeeId { get; init; }
    public int DeductionTypeId { get; init; }
    public decimal Amount { get; init; }
    public bool IsActive { get; init; }
    public string Frequency { get; init; } = "Monthly";
    public string? Remarks { get; init; }
    public string EmployeeCode { get; init; } = string.Empty;
    public string EmployeeName { get; init; } = string.Empty;
    public string DeductionTypeName { get; init; } = string.Empty;
    public bool IsMandatory { get; init; }
    public decimal DefaultAmount { get; init; }
    public int? DepartmentId { get; init; }
    public string? DepartmentName { get; init; }
    public int? BranchId { get; init; }
    public string? BranchName { get; init; }
    public string? PositionTitle { get; init; }
}

public sealed class CreateEmployeeAllowanceDto
{
    public int EmployeeId { get; init; }
    public int AllowanceTypeId { get; init; }
    public decimal Amount { get; init; }
    public bool IsActive { get; init; } = true;
    public string Frequency { get; init; } = "Monthly";
    public string? Remarks { get; init; }
}

public sealed class UpdateEmployeeAllowanceDto
{
    public decimal Amount { get; init; }
    public bool IsActive { get; init; }
    public string Frequency { get; init; } = "Monthly";
    public string? Remarks { get; init; }
}

public sealed class CreateEmployeeDeductionDto
{
    public int EmployeeId { get; init; }
    public int DeductionTypeId { get; init; }
    public decimal Amount { get; init; }
    public bool IsActive { get; init; } = true;
    public string Frequency { get; init; } = "Monthly";
    public string? Remarks { get; init; }
}

public sealed class UpdateEmployeeDeductionDto
{
    public decimal Amount { get; init; }
    public bool IsActive { get; init; }
    public string Frequency { get; init; } = "Monthly";
    public string? Remarks { get; init; }
}
