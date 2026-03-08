namespace PayrollApi.Domain.Entities;

public class LeaveYearEndBatch : BaseEntity
{
    public int Year { get; set; }
    public DateTime ProcessedAt { get; set; }
    public string ProcessedBy { get; set; } = string.Empty;
    public int EmployeesProcessed { get; set; }
    public int BalancesCreated { get; set; }
    public int BalancesExpired { get; set; }
    public int CarryForwardsApplied { get; set; }
    public LeaveYearEndStatus Status { get; set; } = LeaveYearEndStatus.Completed;
    public DateTime? RolledBackAt { get; set; }
    public string? RolledBackBy { get; set; }

    /// <summary>
    /// JSON snapshot of previous balance states for rollback.
    /// </summary>
    public string? SnapshotJson { get; set; }
}

public enum LeaveYearEndStatus
{
    Completed = 1,
    RolledBack = 2
}
