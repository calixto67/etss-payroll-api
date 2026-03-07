namespace PayrollApi.Domain.Entities;

public class LeaveBalance : BaseEntity
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string LeaveType    { get; set; } = string.Empty;
    public int Entitlement     { get; set; }
    public int Used            { get; set; }
    public int Pending         { get; set; }
    public int CarryOver       { get; set; }

    public int Remaining => Entitlement + CarryOver - Used - Pending;
}
