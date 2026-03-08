using System.Text.Json;
using Microsoft.Data.SqlClient;
using PayrollApi.Application.Common.Exceptions;
using PayrollApi.Application.DTOs.Attendance;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class AttendanceService : IAttendanceService
{
    private readonly ISqlExecutor _sql;

    public AttendanceService(ISqlExecutor sql) => _sql = sql;

    // ── Internal row types for Dapper mapping ──────────────────────────

    private sealed class AttendanceRow
    {
        public int Id { get; set; }
        public int PayrollPeriodId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public decimal DaysWorked { get; set; }
        public decimal TotalDays { get; set; }
        public decimal LateHours { get; set; }
        public decimal UndertimeHours { get; set; }
        public decimal OtHours { get; set; }
        public decimal NightDiffHours { get; set; }
        public int Status { get; set; }
        public string? Issue { get; set; }
        public string? ResolutionNotes { get; set; }
        public int? WorkScheduleId { get; set; }
        public string? WorkScheduleName { get; set; }
    }

    private sealed class AttendanceDetailRow
    {
        public int Id { get; set; }
        public int AttendanceId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan? TimeIn { get; set; }
        public TimeSpan? TimeOut { get; set; }
        public decimal LateHours { get; set; }
        public decimal UndertimeHours { get; set; }
        public decimal OtHours { get; set; }
        public decimal NightDiffHours { get; set; }
        public string Status { get; set; } = "";
        public string? Remarks { get; set; }
    }

    // ── Public methods ─────────────────────────────────────────────────

    public async Task<IEnumerable<AttendanceDto>> GetByPeriodAsync(int periodId, string? search, CancellationToken ct)
    {
        try
        {
            var rows = await _sql.QueryAsync<AttendanceRow>(
                "sp_Attendance",
                new { ActionType = "GET_BY_PERIOD", PeriodId = periodId, Search = search },
                ct);

            return rows.Select(r => MapRowToDto(r, null));
        }
        catch (SqlException ex)
        {
            throw new AppException(ex.Message);
        }
    }

    public async Task<AttendanceDto> GetByIdAsync(int id, CancellationToken ct)
    {
        try
        {
            var header = await _sql.QueryFirstOrDefaultAsync<AttendanceRow>(
                "sp_Attendance",
                new { ActionType = "GET_BY_ID", Id = id },
                ct) ?? throw new NotFoundException("Attendance", id);

            var details = await _sql.QueryAsync<AttendanceDetailRow>(
                "sp_Attendance",
                new { ActionType = "GET_DETAILS", Id = id },
                ct);

            return MapRowToDto(header, details.ToList());
        }
        catch (SqlException ex)
        {
            throw new AppException(ex.Message);
        }
    }

    public async Task<AttendanceDto> CreateAsync(int periodId, CreateAttendanceDto dto, string createdBy, CancellationToken ct)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<AttendanceRow>(
                "sp_Attendance",
                new
                {
                    ActionType = "CREATE",
                    PeriodId = periodId,
                    dto.EmployeeId,
                    dto.EmployeeCode,
                    dto.DaysWorked,
                    dto.TotalDays,
                    dto.LateHours,
                    dto.UndertimeHours,
                    dto.OtHours,
                    dto.NightDiffHours,
                    dto.Status,
                    dto.Issue,
                    CreatedBy = createdBy
                },
                ct) ?? throw new AppException("Failed to create attendance record.");

            return await GetByIdAsync(row.Id, ct);
        }
        catch (SqlException ex)
        {
            throw new AppException(ex.Message);
        }
    }

    public async Task<AttendanceDto> UpdateAsync(int id, UpdateAttendanceDto dto, string updatedBy, CancellationToken ct)
    {
        try
        {
            await _sql.ExecuteAsync(
                "sp_Attendance",
                new
                {
                    ActionType = "UPDATE",
                    Id = id,
                    dto.DaysWorked,
                    dto.TotalDays,
                    dto.LateHours,
                    dto.UndertimeHours,
                    dto.OtHours,
                    dto.NightDiffHours,
                    dto.Status,
                    dto.Issue,
                    UpdatedBy = updatedBy
                },
                ct);

            return await GetByIdAsync(id, ct);
        }
        catch (SqlException ex)
        {
            throw new AppException(ex.Message);
        }
    }

    public async Task<AttendanceDto> ResolveAsync(int id, ResolveAttendanceDto dto, string updatedBy, CancellationToken ct)
    {
        try
        {
            await _sql.ExecuteAsync(
                "sp_Attendance",
                new
                {
                    ActionType = "RESOLVE",
                    Id = id,
                    dto.Resolution,
                    dto.Notes,
                    dto.DaysWorked,
                    dto.LateHours,
                    dto.UndertimeHours,
                    dto.OtHours,
                    dto.NightDiffHours,
                    UpdatedBy = updatedBy
                },
                ct);

            return await GetByIdAsync(id, ct);
        }
        catch (SqlException ex)
        {
            throw new AppException(ex.Message);
        }
    }

    public async Task DeleteAsync(int id, string deletedBy, CancellationToken ct)
    {
        try
        {
            await _sql.ExecuteAsync(
                "sp_Attendance",
                new { ActionType = "DELETE", Id = id, DeletedBy = deletedBy },
                ct);
        }
        catch (SqlException ex)
        {
            throw new AppException(ex.Message);
        }
    }

    public async Task<int> ImportAsync(int periodId, IEnumerable<ImportAttendanceRowDto> rows, string createdBy, CancellationToken ct)
    {
        try
        {
            var rowsJson = JsonSerializer.Serialize(rows);

            var count = await _sql.ExecuteScalarAsync<int>(
                "sp_Attendance",
                new { ActionType = "IMPORT", PeriodId = periodId, RowsJson = rowsJson, CreatedBy = createdBy },
                ct);

            return count;
        }
        catch (SqlException ex)
        {
            throw new AppException(ex.Message);
        }
    }

    public async Task<IEnumerable<AttendanceDetailDto>> GetDetailsAsync(int attendanceId, CancellationToken ct)
    {
        try
        {
            var rows = await _sql.QueryAsync<AttendanceDetailRow>(
                "sp_Attendance",
                new { ActionType = "GET_DETAILS", Id = attendanceId },
                ct);

            return rows.Select(MapDetailRowToDto);
        }
        catch (SqlException ex)
        {
            throw new AppException(ex.Message);
        }
    }

    public async Task<AttendanceDto> UpdateDetailsAsync(int attendanceId, IEnumerable<UpdateAttendanceDetailDto> details, string updatedBy, CancellationToken ct)
    {
        try
        {
            var detailsJson = JsonSerializer.Serialize(details);

            await _sql.ExecuteAsync(
                "sp_Attendance",
                new { ActionType = "UPDATE_DETAILS", Id = attendanceId, DetailsJson = detailsJson, UpdatedBy = updatedBy },
                ct);

            return await GetByIdAsync(attendanceId, ct);
        }
        catch (SqlException ex)
        {
            throw new AppException(ex.Message);
        }
    }

    public async Task<int> ImportRawPunchesAsync(int periodId, IEnumerable<ImportRawPunchDto> punches, string createdBy, CancellationToken ct)
    {
        try
        {
            var punchesJson = JsonSerializer.Serialize(punches);

            var count = await _sql.ExecuteScalarAsync<int>(
                "sp_Attendance",
                new { ActionType = "IMPORT_RAW_PUNCHES", PeriodId = periodId, PunchesJson = punchesJson, CreatedBy = createdBy },
                ct);

            return count;
        }
        catch (SqlException ex)
        {
            throw new AppException(ex.Message);
        }
    }

    public async Task<AttendanceDto> ReprocessDetailsAsync(int attendanceId, string updatedBy, CancellationToken ct)
    {
        try
        {
            await _sql.ExecuteAsync(
                "sp_Attendance",
                new { ActionType = "REPROCESS_DETAILS", Id = attendanceId, UpdatedBy = updatedBy },
                ct);

            return await GetByIdAsync(attendanceId, ct);
        }
        catch (SqlException ex)
        {
            throw new AppException(ex.Message);
        }
    }

    public async Task<IEnumerable<ScheduleCheckResultDto>> CheckEmployeeSchedulesAsync(IEnumerable<string> employeeCodes, CancellationToken ct)
    {
        try
        {
            var csv = string.Join(",", employeeCodes.Distinct());

            var rows = await _sql.QueryAsync<ScheduleCheckResultDto>(
                "sp_Attendance",
                new { ActionType = "CHECK_SCHEDULES", EmployeeCodes = csv },
                ct);

            return rows;
        }
        catch (SqlException ex)
        {
            throw new AppException(ex.Message);
        }
    }

    public async Task<IEnumerable<EmployeeScheduleDayDto>> GetEmployeeScheduleAsync(int employeeId, CancellationToken ct)
    {
        try
        {
            var rows = await _sql.QueryAsync<EmployeeScheduleDayDto>(
                "sp_Attendance",
                new { ActionType = "GET_EMPLOYEE_SCHEDULE", EmployeeId = employeeId },
                ct);

            return rows;
        }
        catch (SqlException ex)
        {
            throw new AppException(ex.Message);
        }
    }

    // ── Mapping helpers ────────────────────────────────────────────────

    private static string StatusIntToString(int status) => status switch
    {
        1 => "complete",
        2 => "review",
        3 => "absent",
        _ => "unknown"
    };

    private static AttendanceDto MapRowToDto(AttendanceRow r, List<AttendanceDetailRow>? details) => new(
        r.Id,
        r.PayrollPeriodId,
        r.EmployeeId,
        r.EmployeeCode,
        r.EmployeeName,
        r.DaysWorked,
        r.TotalDays,
        r.LateHours,
        r.UndertimeHours,
        r.OtHours,
        r.NightDiffHours,
        StatusIntToString(r.Status),
        r.Issue,
        r.ResolutionNotes,
        r.WorkScheduleId,
        r.WorkScheduleName,
        details?.Select(MapDetailRowToDto).ToList());

    private static AttendanceDetailDto MapDetailRowToDto(AttendanceDetailRow d) => new(
        d.Id,
        d.AttendanceId,
        d.Date,
        d.TimeIn?.ToString(@"hh\:mm"),
        d.TimeOut?.ToString(@"hh\:mm"),
        d.LateHours,
        d.UndertimeHours,
        d.OtHours,
        d.NightDiffHours,
        d.Status,
        d.Remarks);
}
