namespace PayrollApi.Domain.Entities;

public class OvertimeApplication : BaseEntity
{
    public string ReferenceNumber { get; set; } = string.Empty;
    public string EmployeeName    { get; set; } = string.Empty;
    public string EmployeeCode    { get; set; } = string.Empty;
    public int? PayrollPeriodId   { get; set; }
    public DateTime OvertimeDate  { get; set; }
    public TimeSpan StartTime     { get; set; }
    public TimeSpan EndTime       { get; set; }
    public decimal TotalHours     { get; set; }
    public string Reason          { get; set; } = string.Empty;
    public OvertimeApplicationStatus Status { get; set; } = OvertimeApplicationStatus.Pending;
    public string? ApproverName    { get; set; }
    public string? ApproverRemarks { get; set; }
    public DateTime SubmittedOn    { get; set; } = DateTime.Now;
}

public enum OvertimeApplicationStatus
{
    Pending   = 1,
    Approved  = 2,
    Rejected  = 3,
    Cancelled = 4
}
