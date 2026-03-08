using Microsoft.Data.SqlClient;
using PayrollApi.Application.Common.Exceptions;
using PayrollApi.Application.DTOs.WorkSchedule;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class ScheduleRuleService : IScheduleRuleService
{
    private const string SP = "sp_ScheduleRule";
    private readonly ISqlExecutor _sql;

    public ScheduleRuleService(ISqlExecutor sql) => _sql = sql;

    // ── Internal row class for Dapper mapping ────────────────────────

    private class ScheduleRuleRow
    {
        public int Id { get; set; }
        public decimal HalfDayThresholdHours { get; set; }
        public TimeSpan NightDiffStartTime { get; set; }
        public TimeSpan NightDiffEndTime { get; set; }
        public decimal NightDiffRate { get; set; }
        public int OTMinimumMinutes { get; set; }
        public bool OTRequiresApproval { get; set; }
        public int OTStartAfterMinutes { get; set; }
        public int GracePeriodMinutes { get; set; }
        public int BreakDurationMinutes { get; set; }
        public decimal RegularHoursPerDay { get; set; }
    }

    // ── Row → DTO mapper ─────────────────────────────────────────────

    private static ScheduleRuleDto MapRow(ScheduleRuleRow r) => new()
    {
        Id                    = r.Id,
        HalfDayThresholdHours = r.HalfDayThresholdHours,
        NightDiffStartTime    = r.NightDiffStartTime.ToString(@"hh\:mm"),
        NightDiffEndTime      = r.NightDiffEndTime.ToString(@"hh\:mm"),
        NightDiffRate         = r.NightDiffRate,
        OTMinimumMinutes      = r.OTMinimumMinutes,
        OTRequiresApproval    = r.OTRequiresApproval,
        OTStartAfterMinutes   = r.OTStartAfterMinutes,
        GracePeriodMinutes    = r.GracePeriodMinutes,
        BreakDurationMinutes  = r.BreakDurationMinutes,
        RegularHoursPerDay    = r.RegularHoursPerDay,
    };

    // ── Service methods ──────────────────────────────────────────────

    public async Task<ScheduleRuleDto> GetAsync(CancellationToken ct = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<ScheduleRuleRow>(
                SP,
                new { ActionType = "GET" },
                ct)
                ?? throw new AppException("Schedule rule not found.");

            return MapRow(row);
        }
        catch (SqlException ex)
        {
            throw new AppException($"Failed to retrieve schedule rule: {ex.Message}");
        }
    }

    public async Task<ScheduleRuleDto> UpdateAsync(UpdateScheduleRuleDto dto, string updatedBy, CancellationToken ct = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<ScheduleRuleRow>(
                SP,
                new
                {
                    ActionType            = "UPDATE",
                    HalfDayThresholdHours = dto.HalfDayThresholdHours,
                    NightDiffStartTime    = dto.NightDiffStartTime,
                    NightDiffEndTime      = dto.NightDiffEndTime,
                    NightDiffRate         = dto.NightDiffRate,
                    OTMinimumMinutes      = dto.OTMinimumMinutes,
                    OTRequiresApproval    = dto.OTRequiresApproval,
                    OTStartAfterMinutes   = dto.OTStartAfterMinutes,
                    GracePeriodMinutes    = dto.GracePeriodMinutes,
                    BreakDurationMinutes  = dto.BreakDurationMinutes,
                    RegularHoursPerDay    = dto.RegularHoursPerDay,
                    UpdatedBy             = updatedBy,
                },
                ct)
                ?? throw new AppException("Failed to update schedule rule.");

            return MapRow(row);
        }
        catch (SqlException ex)
        {
            throw new AppException($"Failed to update schedule rule: {ex.Message}");
        }
    }
}
