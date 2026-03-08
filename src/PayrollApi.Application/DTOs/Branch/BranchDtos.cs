namespace PayrollApi.Application.DTOs.Branch;

public sealed class BranchDto
{
    public int Id { get; init; }
    public string BranchCode { get; init; } = string.Empty;
    public string BranchName { get; init; } = string.Empty;
    public string? Address { get; init; }
    public bool IsHeadOffice { get; init; }
}

public sealed class CreateBranchDto
{
    public string BranchCode { get; init; } = string.Empty;
    public string BranchName { get; init; } = string.Empty;
    public string? Address { get; init; }
    public bool IsHeadOffice { get; init; }
}

public sealed class UpdateBranchDto
{
    public string BranchCode { get; init; } = string.Empty;
    public string BranchName { get; init; } = string.Empty;
    public string? Address { get; init; }
    public bool IsHeadOffice { get; init; }
}
