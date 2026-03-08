using Microsoft.Data.SqlClient;
using PayrollApi.Application.Common.Exceptions;
using PayrollApi.Application.DTOs.LeaveType;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class LeaveTypeService : ILeaveTypeService
{
    private const string SP = "sp_LeaveType";

    private readonly ISqlExecutor _sql;

    public LeaveTypeService(ISqlExecutor sql) => _sql = sql;

    // ── Internal row type for Dapper mapping ─────────────────────────────

    private sealed class LeaveTypeRow
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Code { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public decimal DefaultDaysPerYear { get; set; }
        public int EntitlementBasis { get; set; }
        public decimal? TenureIncrementDays { get; set; }
        public decimal? TenureMaxDays { get; set; }
        public string? EligibleEmploymentTypes { get; set; }
        public int AccrualMethod { get; set; }
        public int PayCategory { get; set; }
        public decimal? PaidPercentage { get; set; }
        public int BalanceDeductionMode { get; set; }
        public int CarryForwardPolicy { get; set; }
        public decimal? CarryForwardMaxDays { get; set; }
        public int MinimumNoticeDays { get; set; }
        public bool RequiresApproval { get; set; }
        public bool RequiresAttachment { get; set; }
        public int Granularity { get; set; }
        public bool CountWeekendsAsLeave { get; set; }
        public bool CountHolidaysAsLeave { get; set; }
        public bool AllowCashConversion { get; set; }
        public decimal? MaxCashConversionDays { get; set; }
        public int? GenderRestriction { get; set; }
        public int? MinServiceMonths { get; set; }
    }

    // ── Public methods ───────────────────────────────────────────────────

    public async Task<IEnumerable<LeaveTypeDto>> GetAllAsync(CancellationToken ct = default)
    {
        try
        {
            var rows = await _sql.QueryAsync<LeaveTypeRow>(SP, new { ActionType = "GET_ALL" }, ct);
            return rows.Select(ToDto);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<IEnumerable<LeaveTypeDto>> GetAllActiveAsync(CancellationToken ct = default)
    {
        try
        {
            var rows = await _sql.QueryAsync<LeaveTypeRow>(SP, new { ActionType = "GET_ALL_ACTIVE" }, ct);
            return rows.Select(ToDto);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<LeaveTypeDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<LeaveTypeRow>(
                SP, new { ActionType = "GET_BY_ID", Id = id }, ct)
                ?? throw new AppException($"Leave type {id} not found.");
            return ToDto(row);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<LeaveTypeDto> CreateAsync(CreateLeaveTypeDto dto, string createdBy, CancellationToken ct = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<LeaveTypeRow>(SP, new
            {
                ActionType              = "CREATE",
                Name                    = dto.Name.Trim(),
                Code                    = dto.Code?.Trim(),
                Description             = dto.Description?.Trim(),
                IsActive                = dto.IsActive,
                DefaultDaysPerYear      = dto.DefaultDaysPerYear,
                EntitlementBasis        = dto.EntitlementBasis,
                TenureIncrementDays     = dto.TenureIncrementDays,
                TenureMaxDays           = dto.TenureMaxDays,
                EligibleEmploymentTypes = dto.EligibleEmploymentTypes?.Trim(),
                AccrualMethod           = dto.AccrualMethod,
                PayCategory             = dto.PayCategory,
                PaidPercentage          = dto.PaidPercentage,
                BalanceDeductionMode    = dto.BalanceDeductionMode,
                CarryForwardPolicy      = dto.CarryForwardPolicy,
                CarryForwardMaxDays     = dto.CarryForwardMaxDays,
                MinimumNoticeDays       = dto.MinimumNoticeDays,
                RequiresApproval        = dto.RequiresApproval,
                RequiresAttachment      = dto.RequiresAttachment,
                Granularity             = dto.Granularity,
                CountWeekendsAsLeave    = dto.CountWeekendsAsLeave,
                CountHolidaysAsLeave    = dto.CountHolidaysAsLeave,
                AllowCashConversion     = dto.AllowCashConversion,
                MaxCashConversionDays   = dto.MaxCashConversionDays,
                GenderRestriction       = dto.GenderRestriction,
                MinServiceMonths        = dto.MinServiceMonths,
                CreatedBy               = createdBy,
            }, ct) ?? throw new AppException("Failed to create leave type.");

            return ToDto(row);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<LeaveTypeDto> UpdateAsync(int id, UpdateLeaveTypeDto dto, string updatedBy, CancellationToken ct = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<LeaveTypeRow>(SP, new
            {
                ActionType              = "UPDATE",
                Id                      = id,
                Name                    = dto.Name.Trim(),
                Code                    = dto.Code?.Trim(),
                Description             = dto.Description?.Trim(),
                IsActive                = dto.IsActive,
                DefaultDaysPerYear      = dto.DefaultDaysPerYear,
                EntitlementBasis        = dto.EntitlementBasis,
                TenureIncrementDays     = dto.TenureIncrementDays,
                TenureMaxDays           = dto.TenureMaxDays,
                EligibleEmploymentTypes = dto.EligibleEmploymentTypes?.Trim(),
                AccrualMethod           = dto.AccrualMethod,
                PayCategory             = dto.PayCategory,
                PaidPercentage          = dto.PaidPercentage,
                BalanceDeductionMode    = dto.BalanceDeductionMode,
                CarryForwardPolicy      = dto.CarryForwardPolicy,
                CarryForwardMaxDays     = dto.CarryForwardMaxDays,
                MinimumNoticeDays       = dto.MinimumNoticeDays,
                RequiresApproval        = dto.RequiresApproval,
                RequiresAttachment      = dto.RequiresAttachment,
                Granularity             = dto.Granularity,
                CountWeekendsAsLeave    = dto.CountWeekendsAsLeave,
                CountHolidaysAsLeave    = dto.CountHolidaysAsLeave,
                AllowCashConversion     = dto.AllowCashConversion,
                MaxCashConversionDays   = dto.MaxCashConversionDays,
                GenderRestriction       = dto.GenderRestriction,
                MinServiceMonths        = dto.MinServiceMonths,
                UpdatedBy               = updatedBy,
            }, ct) ?? throw new AppException($"Leave type {id} not found.");

            return ToDto(row);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task DeleteAsync(int id, string deletedBy, CancellationToken ct = default)
    {
        try
        {
            await _sql.ExecuteAsync(SP, new
            {
                ActionType = "DELETE",
                Id         = id,
                DeletedBy  = deletedBy,
            }, ct);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    // ── Helpers ──────────────────────────────────────────────────────────

    private static LeaveTypeDto ToDto(LeaveTypeRow r) => new()
    {
        Id                      = r.Id,
        Name                    = r.Name,
        Code                    = r.Code,
        Description             = r.Description,
        IsActive                = r.IsActive,
        DefaultDaysPerYear      = r.DefaultDaysPerYear,
        EntitlementBasis        = r.EntitlementBasis,
        TenureIncrementDays     = r.TenureIncrementDays,
        TenureMaxDays           = r.TenureMaxDays,
        EligibleEmploymentTypes = r.EligibleEmploymentTypes,
        AccrualMethod           = r.AccrualMethod,
        PayCategory             = r.PayCategory,
        PaidPercentage          = r.PaidPercentage,
        BalanceDeductionMode    = r.BalanceDeductionMode,
        CarryForwardPolicy      = r.CarryForwardPolicy,
        CarryForwardMaxDays     = r.CarryForwardMaxDays,
        MinimumNoticeDays       = r.MinimumNoticeDays,
        RequiresApproval        = r.RequiresApproval,
        RequiresAttachment      = r.RequiresAttachment,
        Granularity             = r.Granularity,
        CountWeekendsAsLeave    = r.CountWeekendsAsLeave,
        CountHolidaysAsLeave    = r.CountHolidaysAsLeave,
        AllowCashConversion     = r.AllowCashConversion,
        MaxCashConversionDays   = r.MaxCashConversionDays,
        GenderRestriction       = r.GenderRestriction,
        MinServiceMonths        = r.MinServiceMonths,
    };
}
