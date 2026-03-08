namespace PayrollApi.Domain.Entities;

public class DeductionType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsMandatory { get; set; }
    public decimal DefaultAmount { get; set; }
    public bool IsActive { get; set; } = true;
}
