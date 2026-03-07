namespace PayrollApi.Domain.Entities;

public class EmployeeEmergencyContact : BaseEntity
{
    public int     EmployeeId    { get; set; }
    public string  ContactName   { get; set; } = string.Empty;
    public string  Relationship  { get; set; } = string.Empty;
    public string  MobileNumber  { get; set; } = string.Empty;
    public string? AlternatePhone { get; set; }
    public bool    IsPrimary     { get; set; } = false;

    public Employee? Employee { get; set; }
}
