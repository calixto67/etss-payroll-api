namespace PayrollApi.Application.DTOs.TaxBracket;

public sealed class TaxBracketDto
{
    public int Id { get; init; }
    public string BracketName { get; init; } = string.Empty;
    public decimal MinAmount { get; init; }
    public decimal? MaxAmount { get; init; }
    public decimal BaseTax { get; init; }
    public decimal TaxRate { get; init; }
    public decimal ExcessOver { get; init; }
    public string EffectiveDate { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}

public sealed class CreateTaxBracketDto
{
    public string BracketName { get; init; } = string.Empty;
    public decimal MinAmount { get; init; }
    public decimal? MaxAmount { get; init; }
    public decimal BaseTax { get; init; }
    public decimal TaxRate { get; init; }
    public decimal ExcessOver { get; init; }
}

public sealed class UpdateTaxBracketDto
{
    public string BracketName { get; init; } = string.Empty;
    public decimal MinAmount { get; init; }
    public decimal? MaxAmount { get; init; }
    public decimal BaseTax { get; init; }
    public decimal TaxRate { get; init; }
    public decimal ExcessOver { get; init; }
}
