namespace PayrollApi.Application.DTOs.Employee;

public sealed class EmergencyContactDto
{
    public int     Id            { get; init; }
    public int     EmployeeId    { get; init; }
    public string  ContactName   { get; init; } = string.Empty;
    public string  Relationship  { get; init; } = string.Empty;
    public string  MobileNumber  { get; init; } = string.Empty;
    public string? AlternatePhone { get; init; }
    public bool    IsPrimary     { get; init; }
}

public sealed class CreateEmergencyContactDto
{
    public string  ContactName   { get; init; } = string.Empty;
    public string  Relationship  { get; init; } = string.Empty;
    public string  MobileNumber  { get; init; } = string.Empty;
    public string? AlternatePhone { get; init; }
    public bool    IsPrimary     { get; init; } = false;
}

public sealed class UpdateEmergencyContactDto
{
    public string  ContactName   { get; init; } = string.Empty;
    public string  Relationship  { get; init; } = string.Empty;
    public string  MobileNumber  { get; init; } = string.Empty;
    public string? AlternatePhone { get; init; }
    public bool    IsPrimary     { get; init; }
}
