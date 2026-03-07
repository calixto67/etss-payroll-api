namespace PayrollApi.Domain.Entities;

/// <summary>
/// Immutable audit log of every employee status transition.
/// Records are never updated or deleted — only appended.
/// </summary>
public class EmployeeStatusHistory : BaseEntity
{
    public int             EmployeeId     { get; set; }
    public EmploymentStatus PreviousStatus { get; set; }
    public EmploymentStatus NewStatus      { get; set; }
    public string          Remarks        { get; set; } = string.Empty;
    public string          ChangedBy      { get; set; } = string.Empty;
    public DateTime        ChangedAt      { get; set; }

    public Employee? Employee { get; set; }
}
