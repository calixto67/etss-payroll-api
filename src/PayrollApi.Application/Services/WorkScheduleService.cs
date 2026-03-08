using System.Text.Json;
using Microsoft.Data.SqlClient;
using PayrollApi.Application.Common.Exceptions;
using PayrollApi.Application.DTOs.WorkSchedule;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class WorkScheduleService : IWorkScheduleService
{
    private const string SP = "sp_WorkSchedule";
    private readonly ISqlExecutor _sql;

    public WorkScheduleService(ISqlExecutor sql) => _sql = sql;

    // ── Internal row classes for Dapper mapping ──────────────────────

    private class ScheduleRow
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public bool IsDefault { get; set; }
        public int EmployeeCount { get; set; }
    }

    private class ScheduleWithDayRow
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public bool IsDefault { get; set; }
        public int EmployeeCount { get; set; }
        // Schedule rule columns
        public decimal RegularHoursPerDay { get; set; }
        public decimal HalfDayThresholdHours { get; set; }
        public int GracePeriodMinutes { get; set; }
        public int BreakDurationMinutes { get; set; }
        public TimeSpan? NightDiffStartTime { get; set; }
        public TimeSpan? NightDiffEndTime { get; set; }
        public decimal NightDiffRate { get; set; }
        public int OTMinimumMinutes { get; set; }
        public int OTStartAfterMinutes { get; set; }
        public bool OTRequiresApproval { get; set; }
        public bool AllowNightDifferential { get; set; }
        public bool AllowOvertime { get; set; }
        // Day columns (nullable for schedules with no days)
        public int? DayId { get; set; }
        public int? DayOfWeek { get; set; }
        public bool? IsRestDay { get; set; }
        public TimeSpan? ShiftStart { get; set; }
        public TimeSpan? ShiftEnd { get; set; }
        public TimeSpan? BreakStart { get; set; }
        public TimeSpan? BreakEnd { get; set; }
    }

    private class EmployeeScheduleRow
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = "";
        public string EmployeeCode { get; set; } = "";
        public int WorkScheduleId { get; set; }
        public string WorkScheduleName { get; set; } = "";
        public DateTime EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    // ── Row → DTO mappers ────────────────────────────────────────────

    private static string? FormatTime(TimeSpan? ts) =>
        ts?.ToString(@"hh\:mm");

    private static WorkScheduleDto GroupToDto(int id, IEnumerable<ScheduleWithDayRow> rows)
    {
        var first = rows.First();
        return new WorkScheduleDto
        {
            Id                    = first.Id,
            Name                  = first.Name,
            Description           = first.Description,
            IsDefault             = first.IsDefault,
            EmployeeCount         = first.EmployeeCount,
            RegularHoursPerDay    = first.RegularHoursPerDay,
            HalfDayThresholdHours = first.HalfDayThresholdHours,
            GracePeriodMinutes    = first.GracePeriodMinutes,
            BreakDurationMinutes  = first.BreakDurationMinutes,
            NightDiffStartTime    = FormatTime(first.NightDiffStartTime),
            NightDiffEndTime      = FormatTime(first.NightDiffEndTime),
            NightDiffRate         = first.NightDiffRate,
            OTMinimumMinutes      = first.OTMinimumMinutes,
            OTStartAfterMinutes   = first.OTStartAfterMinutes,
            OTRequiresApproval    = first.OTRequiresApproval,
            AllowNightDifferential = first.AllowNightDifferential,
            AllowOvertime          = first.AllowOvertime,
            Days = rows
                .Where(r => r.DayId.HasValue)
                .OrderBy(r => r.DayOfWeek)
                .Select(r => new WorkScheduleDayDto
                {
                    Id         = r.DayId!.Value,
                    DayOfWeek  = r.DayOfWeek!.Value,
                    IsRestDay  = r.IsRestDay ?? false,
                    ShiftStart = FormatTime(r.ShiftStart),
                    ShiftEnd   = FormatTime(r.ShiftEnd),
                    BreakStart = FormatTime(r.BreakStart),
                    BreakEnd   = FormatTime(r.BreakEnd),
                }).ToList(),
        };
    }

    private static EmployeeScheduleDto MapEmployee(EmployeeScheduleRow r) => new()
    {
        Id               = r.Id,
        EmployeeId       = r.EmployeeId,
        EmployeeName     = r.EmployeeName,
        EmployeeCode     = r.EmployeeCode,
        WorkScheduleId   = r.WorkScheduleId,
        WorkScheduleName = r.WorkScheduleName,
        EffectiveDate    = r.EffectiveDate,
        EndDate          = r.EndDate,
    };

    private static TimeSpan ParseTime(string? time) =>
        TimeSpan.TryParse(time, out var ts) ? ts : default;

    // ── Service methods ──────────────────────────────────────────────

    public async Task<IEnumerable<WorkScheduleDto>> GetAllAsync(CancellationToken ct = default)
    {
        try
        {
            var rows = await _sql.QueryAsync<ScheduleWithDayRow>(
                SP,
                new { ActionType = "GET_ALL" },
                ct);

            return rows
                .GroupBy(r => r.Id)
                .Select(g => GroupToDto(g.Key, g));
        }
        catch (SqlException ex)
        {
            throw new AppException($"Failed to retrieve work schedules: {ex.Message}");
        }
    }

    public async Task<WorkScheduleDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var rows = await _sql.QueryAsync<ScheduleWithDayRow>(
                SP,
                new { ActionType = "GET_BY_ID", Id = id },
                ct);

            var list = rows.ToList();
            if (list.Count == 0)
                throw new AppException($"Work schedule {id} not found.");

            return GroupToDto(id, list);
        }
        catch (SqlException ex)
        {
            throw new AppException($"Failed to retrieve work schedule: {ex.Message}");
        }
    }

    public async Task<WorkScheduleDto> CreateAsync(CreateWorkScheduleDto dto, string createdBy, CancellationToken ct = default)
    {
        try
        {
            var daysJson = JsonSerializer.Serialize(dto.Days);

            var rows = await _sql.QueryAsync<ScheduleWithDayRow>(
                SP,
                new
                {
                    ActionType             = "CREATE",
                    Name                   = dto.Name.Trim(),
                    Description            = dto.Description?.Trim(),
                    IsDefault              = dto.IsDefault,
                    DaysJson               = daysJson,
                    RegularHoursPerDay     = dto.RegularHoursPerDay,
                    HalfDayThresholdHours  = dto.HalfDayThresholdHours,
                    GracePeriodMinutes     = dto.GracePeriodMinutes,
                    BreakDurationMinutes   = dto.BreakDurationMinutes,
                    NightDiffStartTime     = ParseTime(dto.NightDiffStartTime),
                    NightDiffEndTime       = ParseTime(dto.NightDiffEndTime),
                    NightDiffRate          = dto.NightDiffRate,
                    OTMinimumMinutes       = dto.OTMinimumMinutes,
                    OTStartAfterMinutes    = dto.OTStartAfterMinutes,
                    OTRequiresApproval     = dto.OTRequiresApproval,
                    AllowNightDifferential = dto.AllowNightDifferential,
                    AllowOvertime          = dto.AllowOvertime,
                    CreatedBy              = createdBy,
                },
                ct);

            var list = rows.ToList();
            if (list.Count == 0)
                throw new AppException("Failed to create work schedule.");

            return GroupToDto(list.First().Id, list);
        }
        catch (SqlException ex)
        {
            throw new AppException($"Failed to create work schedule: {ex.Message}");
        }
    }

    public async Task<WorkScheduleDto> UpdateAsync(int id, UpdateWorkScheduleDto dto, string updatedBy, CancellationToken ct = default)
    {
        try
        {
            var daysJson = JsonSerializer.Serialize(dto.Days);

            var rows = await _sql.QueryAsync<ScheduleWithDayRow>(
                SP,
                new
                {
                    ActionType             = "UPDATE",
                    Id                     = id,
                    Name                   = dto.Name.Trim(),
                    Description            = dto.Description?.Trim(),
                    IsDefault              = dto.IsDefault,
                    DaysJson               = daysJson,
                    RegularHoursPerDay     = dto.RegularHoursPerDay,
                    HalfDayThresholdHours  = dto.HalfDayThresholdHours,
                    GracePeriodMinutes     = dto.GracePeriodMinutes,
                    BreakDurationMinutes   = dto.BreakDurationMinutes,
                    NightDiffStartTime     = ParseTime(dto.NightDiffStartTime),
                    NightDiffEndTime       = ParseTime(dto.NightDiffEndTime),
                    NightDiffRate          = dto.NightDiffRate,
                    OTMinimumMinutes       = dto.OTMinimumMinutes,
                    OTStartAfterMinutes    = dto.OTStartAfterMinutes,
                    OTRequiresApproval     = dto.OTRequiresApproval,
                    AllowNightDifferential = dto.AllowNightDifferential,
                    AllowOvertime          = dto.AllowOvertime,
                    UpdatedBy              = updatedBy,
                },
                ct);

            var list = rows.ToList();
            if (list.Count == 0)
                throw new AppException($"Work schedule {id} not found.");

            return GroupToDto(id, list);
        }
        catch (SqlException ex)
        {
            throw new AppException($"Failed to update work schedule: {ex.Message}");
        }
    }

    public async Task DeleteAsync(int id, string deletedBy, CancellationToken ct = default)
    {
        try
        {
            await _sql.ExecuteAsync(
                SP,
                new { ActionType = "DELETE", Id = id, DeletedBy = deletedBy },
                ct);
        }
        catch (SqlException ex)
        {
            throw new AppException($"Failed to delete work schedule: {ex.Message}");
        }
    }

    public async Task<IEnumerable<EmployeeScheduleDto>> AssignEmployeesAsync(
        int scheduleId, AssignEmployeeScheduleDto dto, string assignedBy, CancellationToken ct = default)
    {
        try
        {
            var employeeIdsCsv = string.Join(",", dto.EmployeeIds);

            await _sql.ExecuteAsync(
                SP,
                new
                {
                    ActionType    = "ASSIGN_EMPLOYEES",
                    Id            = scheduleId,
                    EmployeeIds   = employeeIdsCsv,
                    EffectiveDate = dto.EffectiveDate,
                    CreatedBy     = assignedBy,
                },
                ct);

            return await GetEmployeesAsync(scheduleId, ct);
        }
        catch (SqlException ex)
        {
            throw new AppException($"Failed to assign employees to schedule: {ex.Message}");
        }
    }

    public async Task<IEnumerable<EmployeeScheduleDto>> GetEmployeesAsync(int scheduleId, CancellationToken ct = default)
    {
        try
        {
            var rows = await _sql.QueryAsync<EmployeeScheduleRow>(
                SP,
                new { ActionType = "GET_EMPLOYEES", Id = scheduleId },
                ct);

            return rows.Select(MapEmployee);
        }
        catch (SqlException ex)
        {
            throw new AppException($"Failed to retrieve schedule employees: {ex.Message}");
        }
    }

    public async Task UnassignEmployeeAsync(int scheduleId, int employeeId, string unassignedBy, CancellationToken ct = default)
    {
        try
        {
            await _sql.ExecuteAsync(
                SP,
                new
                {
                    ActionType = "UNASSIGN_EMPLOYEE",
                    Id         = scheduleId,
                    EmployeeId = employeeId,
                    UpdatedBy  = unassignedBy,
                },
                ct);
        }
        catch (SqlException ex)
        {
            throw new AppException($"Failed to unassign employee from schedule: {ex.Message}");
        }
    }
}
