namespace PayrollApi.Application.DTOs.ContributionBracket;

public sealed class ContributionBracketDto
{
    public int Id { get; init; }
    public string ContributionType { get; init; } = string.Empty;
    public decimal RangeFrom { get; init; }
    public decimal? RangeTo { get; init; }
    public decimal EmployeeShare { get; init; }
    public decimal EmployerShare { get; init; }
}

public sealed class CreateContributionBracketDto
{
    public string ContributionType { get; init; } = string.Empty;
    public decimal RangeFrom { get; init; }
    public decimal? RangeTo { get; init; }
    public decimal EmployeeShare { get; init; }
    public decimal EmployerShare { get; init; }
}

public sealed class UpdateContributionBracketDto
{
    public decimal RangeFrom { get; init; }
    public decimal? RangeTo { get; init; }
    public decimal EmployeeShare { get; init; }
    public decimal EmployerShare { get; init; }
}
