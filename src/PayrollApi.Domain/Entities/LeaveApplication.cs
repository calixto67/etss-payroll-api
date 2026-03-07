namespace PayrollApi.Domain.Entities;

public class LeaveApplication : BaseEntity
{
    public string ReferenceNumber { get; set; } = string.Empty;
    public string EmployeeName    { get; set; } = string.Empty;
    public string LeaveType       { get; set; } = string.Empty;
    public DateTime StartDate     { get; set; }
    public DateTime EndDate       { get; set; }
    public int DeductibleDays     { get; set; }
    public string Reason          { get; set; } = string.Empty;
    public LeaveApplicationStatus Status { get; set; } = LeaveApplicationStatus.Pending;
    public DateTime SubmittedOn   { get; set; } = DateTime.UtcNow;
    public string ApproverName    { get; set; } = string.Empty;
    public string? ApproverRemarks { get; set; }
}

public enum LeaveApplicationStatus
{
    Pending   = 1,
    Approved  = 2,
    Rejected  = 3,
    Cancelled = 4
}
