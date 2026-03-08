namespace PayrollApi.Domain.Entities;

public class AllowanceType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public bool IsDeMinimis { get; set; }
    public decimal TaxExemptLimit { get; set; }
}
