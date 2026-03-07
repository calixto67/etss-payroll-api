namespace PayrollApi.Application.DTOs.CompanySettings;

public sealed class CompanySettingsDto
{
    public string CompanyName { get; init; } = string.Empty;

    /// <summary>Null when no logo has been uploaded.</summary>
    public string? LogoUrl { get; init; }
}

public sealed class UpdateCompanySettingsDto
{
    public string CompanyName { get; init; } = string.Empty;
}
