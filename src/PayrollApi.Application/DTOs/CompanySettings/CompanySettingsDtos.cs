namespace PayrollApi.Application.DTOs.CompanySettings;

public sealed class CompanySettingsDto
{
    public string CompanyName { get; init; } = string.Empty;
    public string? Address { get; init; }
    public string? DateStarted { get; init; }
    public string? TaxNo { get; init; }
    public string? BirNo { get; init; }
    public string? EmployerSssNo { get; init; }
    public string? IndustryClassification { get; init; }

    /// <summary>Null when no logo has been uploaded.</summary>
    public string? LogoUrl { get; init; }

    public string DateFormat { get; init; } = "MM/dd/yyyy";

    // Government Mandate Default Contributions
    public decimal DefaultSssContribution { get; init; } = 900m;
    public decimal DefaultPhilHealthContribution { get; init; } = 500m;
    public decimal DefaultPagIbigContribution { get; init; } = 200m;
}

public sealed class UpdateCompanySettingsDto
{
    public string CompanyName { get; init; } = string.Empty;
    public string? Address { get; init; }
    public string? DateStarted { get; init; }
    public string? TaxNo { get; init; }
    public string? BirNo { get; init; }
    public string? EmployerSssNo { get; init; }
    public string? IndustryClassification { get; init; }
    public string? DateFormat { get; init; }
}

public sealed class UpdateDeductionSettingsDto
{
    public decimal DefaultSssContribution { get; init; }
    public decimal DefaultPhilHealthContribution { get; init; }
    public decimal DefaultPagIbigContribution { get; init; }
}
