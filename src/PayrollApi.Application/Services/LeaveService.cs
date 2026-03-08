using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using PayrollApi.Application.Common.Exceptions;
using PayrollApi.Application.DTOs.Leave;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class LeaveService : ILeaveService
{
    private readonly ISqlExecutor _sql;
    private readonly ILogger<LeaveService> _logger;

    public LeaveService(ISqlExecutor sql, ILogger<LeaveService> logger)
    {
        _sql    = sql;
        _logger = logger;
    }

    // ── Internal row classes for Dapper mapping ─────────────────────

    private class ApplicationRow
    {
        public int Id { get; set; }
        public string ReferenceNumber { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string LeaveType { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DeductibleDays { get; set; }
        public string? Reason { get; set; }
        public int Status { get; set; }
        public DateTime SubmittedOn { get; set; }
        public string? ApproverName { get; set; }
        public string? ApproverRemarks { get; set; }
    }

    private class HolidayRow
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime Date { get; set; }
        public int Type { get; set; }
        public string Region { get; set; } = "";
        public bool IsRecurring { get; set; }
    }

    private class YearEndBatchRow
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public DateTime ProcessedAt { get; set; }
        public string ProcessedBy { get; set; } = "";
        public int EmployeesProcessed { get; set; }
        public int BalancesCreated { get; set; }
        public int BalancesExpired { get; set; }
        public int CarryForwardsApplied { get; set; }
        public int Status { get; set; }
    }

    // ── Status/Type name helpers ────────────────────────────────────

    private static string ApplicationStatusName(int status) => status switch
    {
        1 => "Pending",
        2 => "Approved",
        3 => "Rejected",
        4 => "Cancelled",
        _ => "Unknown"
    };

    private static string HolidayTypeName(int type) => type switch
    {
        1 => "Public",
        2 => "Company",
        3 => "Regional",
        4 => "Special",
        _ => "Unknown"
    };

    private static string YearEndStatusName(int status) => status switch
    {
        1 => "Completed",
        2 => "RolledBack",
        _ => "Unknown"
    };

    // ── Row → DTO mappers ───────────────────────────────────────────

    private static LeaveApplicationDto MapApplication(ApplicationRow r) => new(
        r.Id, r.ReferenceNumber, r.EmployeeName, r.LeaveType,
        r.StartDate.ToString("yyyy-MM-dd"), r.EndDate.ToString("yyyy-MM-dd"),
        r.DeductibleDays, r.Reason, ApplicationStatusName(r.Status),
        r.SubmittedOn.ToString("yyyy-MM-dd"),
        r.ApproverName, r.ApproverRemarks);

    private static HolidayDto MapHoliday(HolidayRow r) => new(
        r.Id, r.Name, r.Date.ToString("yyyy-MM-dd"),
        HolidayTypeName(r.Type), r.Region, r.IsRecurring);

    private static LeaveYearEndBatchDto MapBatch(YearEndBatchRow r) => new(
        r.Id, r.Year, r.ProcessedAt.ToString("yyyy-MM-dd HH:mm:ss"),
        r.ProcessedBy, r.EmployeesProcessed, r.BalancesCreated,
        r.BalancesExpired, r.CarryForwardsApplied, YearEndStatusName(r.Status));

    // ── Applications ────────────────────────────────────────────────

    public async Task<IEnumerable<LeaveApplicationDto>> GetApplicationsAsync(
        string? search, string? status, string? leaveType,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var rows = await _sql.QueryAsync<ApplicationRow>(
                "sp_Leave",
                new { ActionType = "GET_APPLICATIONS", Search = search, Status = status, LeaveType = leaveType },
                cancellationToken);

            return rows.Select(MapApplication);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Failed to get leave applications");
            throw new AppException(ex.Message);
        }
    }

    public async Task<LeaveApplicationDto> CreateApplicationAsync(
        CreateLeaveApplicationDto dto, string createdBy,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<ApplicationRow>(
                "sp_Leave",
                new
                {
                    ActionType = "CREATE_APPLICATION",
                    Employee  = dto.Employee,
                    LeaveType = dto.LeaveType,
                    StartDate = dto.StartDate,
                    EndDate   = dto.EndDate,
                    Reason    = dto.Reason,
                    Approver  = dto.Approver,
                    CreatedBy = createdBy
                },
                cancellationToken)
                ?? throw new AppException("Failed to create leave application.");

            _logger.LogInformation("Leave application {RefNo} created by {User}", row.ReferenceNumber, createdBy);
            return MapApplication(row);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Failed to create leave application");
            throw new AppException(ex.Message);
        }
    }

    public async Task<LeaveApplicationDto> UpdateApplicationStatusAsync(
        int id, UpdateLeaveApplicationStatusDto dto, string updatedBy,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<ApplicationRow>(
                "sp_Leave",
                new { ActionType = "UPDATE_APPLICATION_STATUS", Id = id, Status = dto.Status, Remarks = dto.Remarks, UpdatedBy = updatedBy },
                cancellationToken)
                ?? throw new AppException("Leave application not found or update failed.");

            _logger.LogInformation("Leave application {Id} status updated to {Status} by {User}", id, dto.Status, updatedBy);
            return MapApplication(row);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Failed to update leave application status");
            throw new AppException(ex.Message);
        }
    }

    // ── Balances ────────────────────────────────────────────────────

    public async Task<IEnumerable<LeaveBalanceDto>> GetBalancesAsync(
        string? search, string? leaveType,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var rows = await _sql.QueryAsync<LeaveBalanceDto>(
                "sp_Leave",
                new { ActionType = "GET_BALANCES", Search = search, LeaveType = leaveType },
                cancellationToken);

            return rows;
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Failed to get leave balances");
            throw new AppException(ex.Message);
        }
    }

    public async Task<LeaveBalanceDto> CreateBalanceAsync(
        CreateLeaveBalanceDto dto, string createdBy,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<LeaveBalanceDto>(
                "sp_Leave",
                new
                {
                    ActionType   = "CREATE_BALANCE",
                    EmployeeCode = dto.EmployeeCode,
                    EmployeeName = dto.EmployeeName,
                    LeaveType    = dto.LeaveType,
                    Entitlement  = dto.Entitlement,
                    CarryOver    = dto.CarryOver,
                    CreatedBy    = createdBy
                },
                cancellationToken)
                ?? throw new AppException("Failed to create leave balance.");

            _logger.LogInformation("Leave balance created for {Employee} - {LeaveType} by {User}", dto.EmployeeCode, dto.LeaveType, createdBy);
            return row;
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Failed to create leave balance");
            throw new AppException(ex.Message);
        }
    }

    public async Task<LeaveBalanceDto> UpdateBalanceAsync(
        int id, UpdateLeaveBalanceDto dto, string updatedBy,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<LeaveBalanceDto>(
                "sp_Leave",
                new
                {
                    ActionType  = "UPDATE_BALANCE",
                    Id          = id,
                    Entitlement = dto.Entitlement,
                    Used        = dto.Used,
                    Pending     = dto.Pending,
                    CarryOver   = dto.CarryOver,
                    UpdatedBy   = updatedBy
                },
                cancellationToken)
                ?? throw new AppException("Leave balance not found or update failed.");

            return row;
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Failed to update leave balance");
            throw new AppException(ex.Message);
        }
    }

    public async Task DeleteBalanceAsync(int id, string deletedBy,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _sql.ExecuteAsync(
                "sp_Leave",
                new { ActionType = "DELETE_BALANCE", Id = id, DeletedBy = deletedBy },
                cancellationToken);

            _logger.LogInformation("Leave balance {Id} deleted by {User}", id, deletedBy);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Failed to delete leave balance");
            throw new AppException(ex.Message);
        }
    }

    public async Task<int> EnrollAllEmployeesAsync(
        EnrollAllDto dto, string createdBy,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var count = await _sql.ExecuteScalarAsync<int>(
                "sp_Leave",
                new { ActionType = "ENROLL_ALL", LeaveType = dto.LeaveType, Entitlement = dto.Entitlement, CreatedBy = createdBy },
                cancellationToken);

            _logger.LogInformation("Enrolled {Count} employees in '{LeaveType}' by {User}", count, dto.LeaveType, createdBy);
            return count;
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Failed to enroll all employees");
            throw new AppException(ex.Message);
        }
    }

    // ── Year-End Processing ─────────────────────────────────────────

    public async Task<LeaveYearEndResultDto> RunYearEndProcessingAsync(
        int year, string processedBy,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<LeaveYearEndResultDto>(
                "sp_Leave",
                new { ActionType = "RUN_YEAR_END", Year = year, ProcessedBy = processedBy },
                cancellationToken)
                ?? throw new AppException("Year-end processing returned no result.");

            _logger.LogInformation(
                "Leave year-end processing completed for {Year} by {User}: {Employees} employees, {Balances} balances created",
                year, processedBy, row.EmployeesProcessed, row.BalancesCreated);

            return row;
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Failed to run year-end processing");
            throw new AppException(ex.Message);
        }
    }

    public async Task<LeaveYearEndResultDto> RollbackYearEndProcessingAsync(
        string rolledBackBy,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<LeaveYearEndResultDto>(
                "sp_Leave",
                new { ActionType = "ROLLBACK_YEAR_END", RolledBackBy = rolledBackBy },
                cancellationToken)
                ?? throw new AppException("Rollback returned no result.");

            _logger.LogInformation(
                "Leave year-end processing for {Year} rolled back by {User}",
                row.Year, rolledBackBy);

            return row;
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Failed to rollback year-end processing");
            throw new AppException(ex.Message);
        }
    }

    public async Task<LeaveYearEndBatchDto?> GetLastBatchAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<YearEndBatchRow>(
                "sp_Leave",
                new { ActionType = "GET_LAST_BATCH" },
                cancellationToken);

            return row is null ? null : MapBatch(row);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Failed to get last year-end batch");
            throw new AppException(ex.Message);
        }
    }

    // ── Holidays ────────────────────────────────────────────────────

    public async Task<IEnumerable<HolidayDto>> GetHolidaysAsync(
        string? search, string? type,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var rows = await _sql.QueryAsync<HolidayRow>(
                "sp_Leave",
                new { ActionType = "GET_HOLIDAYS", Search = search, Type = type },
                cancellationToken);

            return rows.Select(MapHoliday);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Failed to get holidays");
            throw new AppException(ex.Message);
        }
    }

    public async Task<HolidayDto> CreateHolidayAsync(
        CreateHolidayDto dto, string createdBy,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<HolidayRow>(
                "sp_Leave",
                new
                {
                    ActionType  = "CREATE_HOLIDAY",
                    Name        = dto.Name,
                    Date        = dto.Date,
                    Type        = dto.Type,
                    Region      = dto.Region,
                    IsRecurring = dto.IsRecurring,
                    CreatedBy   = createdBy
                },
                cancellationToken)
                ?? throw new AppException("Failed to create holiday.");

            _logger.LogInformation("Holiday '{Name}' added by {User}", dto.Name, createdBy);
            return MapHoliday(row);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Failed to create holiday");
            throw new AppException(ex.Message);
        }
    }

    public async Task DeleteHolidayAsync(int id, string deletedBy,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _sql.ExecuteAsync(
                "sp_Leave",
                new { ActionType = "DELETE_HOLIDAY", Id = id, DeletedBy = deletedBy },
                cancellationToken);

            _logger.LogInformation("Holiday {Id} deleted by {User}", id, deletedBy);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Failed to delete holiday");
            throw new AppException(ex.Message);
        }
    }
}
