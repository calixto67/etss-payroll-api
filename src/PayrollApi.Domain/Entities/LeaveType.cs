namespace PayrollApi.Domain.Entities;

public class LeaveType : BaseEntity
{
    // ── Basic ────────────────────────────────────────────────────
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // ── Entitlement ──────────────────────────────────────────────
    public decimal DefaultDaysPerYear { get; set; }
    public LeaveEntitlementBasis EntitlementBasis { get; set; } = LeaveEntitlementBasis.Fixed;
    public decimal? TenureIncrementDays { get; set; }
    public decimal? TenureMaxDays { get; set; }
    public string? EligibleEmploymentTypes { get; set; }

    // ── Accrual ──────────────────────────────────────────────────
    public LeaveAccrualMethod AccrualMethod { get; set; } = LeaveAccrualMethod.Annual;

    // ── Pay Category ─────────────────────────────────────────────
    public LeavePayCategory PayCategory { get; set; } = LeavePayCategory.Paid;
    public decimal? PaidPercentage { get; set; }

    // ── Balance Tracking ─────────────────────────────────────────
    public LeaveBalanceDeductionMode BalanceDeductionMode { get; set; } = LeaveBalanceDeductionMode.Deducts;

    // ── Carry Forward ────────────────────────────────────────────
    public LeaveCarryForwardPolicy CarryForwardPolicy { get; set; } = LeaveCarryForwardPolicy.None;
    public decimal? CarryForwardMaxDays { get; set; }

    // ── Filing Rules ─────────────────────────────────────────────
    public int MinimumNoticeDays { get; set; }
    public bool RequiresApproval { get; set; } = true;
    public bool RequiresAttachment { get; set; }

    // ── Day Granularity ──────────────────────────────────────────
    public LeaveGranularity Granularity { get; set; } = LeaveGranularity.FullDay;

    // ── Weekend/Holiday Counting ─────────────────────────────────
    public bool CountWeekendsAsLeave { get; set; }
    public bool CountHolidaysAsLeave { get; set; }

    // ── Cash Conversion ──────────────────────────────────────────
    public bool AllowCashConversion { get; set; }
    public decimal? MaxCashConversionDays { get; set; }

    // ── Gender Restriction ───────────────────────────────────────
    public Gender? GenderRestriction { get; set; }

    // ── Waiting Period ───────────────────────────────────────────
    public int? MinServiceMonths { get; set; }
}

// ── Enums ─────────────────────────────────────────────────────────

public enum LeaveEntitlementBasis
{
    Fixed = 1,
    Tenure = 2,
    EmploymentType = 3
}

public enum LeaveAccrualMethod
{
    Annual = 1,
    Monthly = 2
}

public enum LeavePayCategory
{
    Paid = 1,
    Unpaid = 2,
    PartiallyPaid = 3
}

public enum LeaveBalanceDeductionMode
{
    Deducts = 1,
    Unlimited = 2,
    DoesNotDeduct = 3
}

public enum LeaveCarryForwardPolicy
{
    None = 1,
    Limited = 2,
    Full = 3
}

public enum LeaveGranularity
{
    FullDay = 1,
    HalfDay = 2,
    Hourly = 3
}
