namespace PayrollApi.Application.DTOs.AllowanceType;

public sealed class AllowanceTypeDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public bool IsDeMinimis { get; init; }
    public decimal TaxExemptLimit { get; init; }
}

public sealed class CreateAllowanceTypeDto
{
    public string Name { get; init; } = string.Empty;
    public bool IsDeMinimis { get; init; }
    public decimal TaxExemptLimit { get; init; }
}

public sealed class UpdateAllowanceTypeDto
{
    public string Name { get; init; } = string.Empty;
    public bool IsDeMinimis { get; init; }
    public decimal TaxExemptLimit { get; init; }
}
