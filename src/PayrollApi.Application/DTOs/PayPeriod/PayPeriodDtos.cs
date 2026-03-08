using PayrollApi.Domain.Entities;

namespace PayrollApi.Application.DTOs.PayPeriod;

public record PayPeriodDto
{
    public int      Id            { get; init; }
    public string   Name          { get; init; } = string.Empty;
    public string   PeriodCode    { get; init; } = string.Empty;
    public DateTime StartDate     { get; init; }
    public DateTime EndDate       { get; init; }
    public DateTime PayDate       { get; init; }
    public string   PeriodType    { get; init; } = string.Empty;
    public string   Status        { get; init; } = string.Empty;
    public int      EmployeeCount { get; init; }
    public bool     IsClosed      { get; init; }
    public DateTime CreatedAt     { get; init; }
}

public record CreatePayPeriodDto(
    DateTime   StartDate,
    DateTime   EndDate,
    DateTime   PayDate,
    PeriodType PeriodType);

/// <summary>Accepts status as a string (e.g. "open", "computed") for easy JSON binding.</summary>
public record UpdatePayPeriodStatusDto(string Status);
