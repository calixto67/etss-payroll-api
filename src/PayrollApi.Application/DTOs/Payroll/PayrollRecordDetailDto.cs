namespace PayrollApi.Application.DTOs.Payroll;

public sealed class PayrollRecordDetailDto
{
    public int Id { get; init; }
    public int PayrollRecordId { get; init; }
    public string ItemType { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public decimal Amount { get; init; }
}
