namespace PayrollApi.Application.DTOs.LeaveType;

public sealed class LeaveTypeDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Code { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; }

    // Entitlement
    public decimal DefaultDaysPerYear { get; init; }
    public int EntitlementBasis { get; init; }
    public decimal? TenureIncrementDays { get; init; }
    public decimal? TenureMaxDays { get; init; }
    public string? EligibleEmploymentTypes { get; init; }

    // Accrual
    public int AccrualMethod { get; init; }

    // Pay
    public int PayCategory { get; init; }
    public decimal? PaidPercentage { get; init; }

    // Balance
    public int BalanceDeductionMode { get; init; }

    // Carry Forward
    public int CarryForwardPolicy { get; init; }
    public decimal? CarryForwardMaxDays { get; init; }

    // Filing
    public int MinimumNoticeDays { get; init; }
    public bool RequiresApproval { get; init; }
    public bool RequiresAttachment { get; init; }

    // Granularity
    public int Granularity { get; init; }

    // Weekend/Holiday
    public bool CountWeekendsAsLeave { get; init; }
    public bool CountHolidaysAsLeave { get; init; }

    // Cash Conversion
    public bool AllowCashConversion { get; init; }
    public decimal? MaxCashConversionDays { get; init; }

    // Gender
    public int? GenderRestriction { get; init; }

    // Waiting Period
    public int? MinServiceMonths { get; init; }
}

public sealed class CreateLeaveTypeDto
{
    public string Name { get; init; } = string.Empty;
    public string? Code { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; } = true;

    public decimal DefaultDaysPerYear { get; init; }
    public int EntitlementBasis { get; init; } = 1;
    public decimal? TenureIncrementDays { get; init; }
    public decimal? TenureMaxDays { get; init; }
    public string? EligibleEmploymentTypes { get; init; }

    public int AccrualMethod { get; init; } = 1;
    public int PayCategory { get; init; } = 1;
    public decimal? PaidPercentage { get; init; }
    public int BalanceDeductionMode { get; init; } = 1;
    public int CarryForwardPolicy { get; init; } = 1;
    public decimal? CarryForwardMaxDays { get; init; }

    public int MinimumNoticeDays { get; init; }
    public bool RequiresApproval { get; init; } = true;
    public bool RequiresAttachment { get; init; }
    public int Granularity { get; init; } = 1;

    public bool CountWeekendsAsLeave { get; init; }
    public bool CountHolidaysAsLeave { get; init; }
    public bool AllowCashConversion { get; init; }
    public decimal? MaxCashConversionDays { get; init; }
    public int? GenderRestriction { get; init; }
    public int? MinServiceMonths { get; init; }
}

public sealed class UpdateLeaveTypeDto
{
    public string Name { get; init; } = string.Empty;
    public string? Code { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; } = true;

    public decimal DefaultDaysPerYear { get; init; }
    public int EntitlementBasis { get; init; } = 1;
    public decimal? TenureIncrementDays { get; init; }
    public decimal? TenureMaxDays { get; init; }
    public string? EligibleEmploymentTypes { get; init; }

    public int AccrualMethod { get; init; } = 1;
    public int PayCategory { get; init; } = 1;
    public decimal? PaidPercentage { get; init; }
    public int BalanceDeductionMode { get; init; } = 1;
    public int CarryForwardPolicy { get; init; } = 1;
    public decimal? CarryForwardMaxDays { get; init; }

    public int MinimumNoticeDays { get; init; }
    public bool RequiresApproval { get; init; } = true;
    public bool RequiresAttachment { get; init; }
    public int Granularity { get; init; } = 1;

    public bool CountWeekendsAsLeave { get; init; }
    public bool CountHolidaysAsLeave { get; init; }
    public bool AllowCashConversion { get; init; }
    public decimal? MaxCashConversionDays { get; init; }
    public int? GenderRestriction { get; init; }
    public int? MinServiceMonths { get; init; }
}
