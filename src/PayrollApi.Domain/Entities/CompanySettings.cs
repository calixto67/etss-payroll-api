namespace PayrollApi.Domain.Entities;

/// <summary>
/// Singleton row (Id = 1) that holds company-wide configuration such as the logo path.
/// </summary>
public class CompanySettings : BaseEntity
{
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>Relative path under wwwroot, e.g. "uploads/logo/company.png".</summary>
    public string? LogoPath { get; set; }
}
