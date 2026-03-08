namespace PayrollApi.Domain.Entities;

/// <summary>
/// Singleton row (Id = 1) that holds company-wide configuration such as the logo path.
/// </summary>
public class CompanySettings : BaseEntity
{
    public string CompanyName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public DateTime? DateStarted { get; set; }
    public string? TaxNo { get; set; }
    public string? BirNo { get; set; }
    public string? EmployerSssNo { get; set; }
    public string? IndustryClassification { get; set; }

    /// <summary>Relative path under wwwroot, e.g. "uploads/logo/company.png".</summary>
    public string? LogoPath { get; set; }

    /// <summary>Date display format, e.g. "MM/dd/yyyy", "dd/MM/yyyy", "yyyy-MM-dd".</summary>
    public string DateFormat { get; set; } = "MM/dd/yyyy";

    // ── Government Mandate Default Contributions ─────────────────────────────
    /// <summary>Default SSS employee contribution amount, e.g. 900.</summary>
    public decimal DefaultSssContribution { get; set; } = 900m;

    /// <summary>Default PhilHealth employee contribution amount, e.g. 500.</summary>
    public decimal DefaultPhilHealthContribution { get; set; } = 500m;

    /// <summary>Default Pag-IBIG employee contribution amount, e.g. 200.</summary>
    public decimal DefaultPagIbigContribution { get; set; } = 200m;
}
