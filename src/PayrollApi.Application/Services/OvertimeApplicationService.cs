using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using PayrollApi.Application.Common.Exceptions;
using PayrollApi.Application.DTOs.OvertimeApplication;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class OvertimeApplicationService : IOvertimeApplicationService
{
    private readonly ISqlExecutor _sql;
    private readonly ILogger<OvertimeApplicationService> _logger;

    public OvertimeApplicationService(ISqlExecutor sql, ILogger<OvertimeApplicationService> logger)
    {
        _sql    = sql;
        _logger = logger;
    }

    private class ApplicationRow
    {
        public int Id { get; set; }
        public string ReferenceNumber { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string EmployeeCode { get; set; } = "";
        public int? PayrollPeriodId { get; set; }
        public DateTime OvertimeDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public decimal TotalHours { get; set; }
        public string Reason { get; set; } = "";
        public int Status { get; set; }
        public string? ApproverName { get; set; }
        public string? ApproverRemarks { get; set; }
        public DateTime SubmittedOn { get; set; }
    }

    private static string StatusName(int status) => status switch
    {
        1 => "Pending",
        2 => "Approved",
        3 => "Rejected",
        4 => "Cancelled",
        _ => "Unknown"
    };

    private static OvertimeApplicationDto Map(ApplicationRow r) => new(
        r.Id,
        r.ReferenceNumber,
        r.EmployeeName,
        r.EmployeeCode,
        r.PayrollPeriodId,
        r.OvertimeDate.ToString("yyyy-MM-dd"),
        r.StartTime.ToString(@"hh\:mm"),
        r.EndTime.ToString(@"hh\:mm"),
        r.TotalHours,
        r.Reason,
        StatusName(r.Status),
        r.ApproverName,
        r.ApproverRemarks,
        r.SubmittedOn.ToString("yyyy-MM-dd"));

    public async Task<IEnumerable<OvertimeApplicationDto>> GetApplicationsAsync(
        string? search, string? status, int? payrollPeriodId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var rows = await _sql.QueryAsync<ApplicationRow>(
                "sp_OvertimeApplication",
                new { ActionType = "GET_APPLICATIONS", Search = search, Status = status, PayrollPeriodId = payrollPeriodId },
                cancellationToken);

            return rows.Select(Map);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Failed to get overtime applications");
            throw new AppException(ex.Message);
        }
    }

    public async Task<OvertimeApplicationDto> CreateApplicationAsync(
        CreateOvertimeApplicationDto dto, string createdBy,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<ApplicationRow>(
                "sp_OvertimeApplication",
                new
                {
                    ActionType      = "CREATE",
                    EmployeeName    = dto.Employee,
                    EmployeeCode    = dto.EmployeeCode,
                    PayrollPeriodId = dto.PayrollPeriodId,
                    OvertimeDate    = dto.OvertimeDate,
                    StartTime       = dto.StartTime,
                    EndTime         = dto.EndTime,
                    TotalHours      = dto.TotalHours,
                    Reason          = dto.Reason,
                    Approver        = dto.Approver,
                    CreatedBy       = createdBy
                },
                cancellationToken)
                ?? throw new AppException("Failed to create overtime application.");

            _logger.LogInformation("Overtime application {RefNo} created by {User}", row.ReferenceNumber, createdBy);
            return Map(row);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Failed to create overtime application");
            throw new AppException(ex.Message);
        }
    }

    public async Task<OvertimeApplicationDto> UpdateApplicationStatusAsync(
        int id, UpdateOvertimeApplicationStatusDto dto, string updatedBy,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<ApplicationRow>(
                "sp_OvertimeApplication",
                new { ActionType = "UPDATE_STATUS", Id = id, Status = dto.Status, Remarks = dto.Remarks, UpdatedBy = updatedBy },
                cancellationToken)
                ?? throw new AppException("Overtime application not found or update failed.");

            _logger.LogInformation("Overtime application {Id} status updated to {Status} by {User}", id, dto.Status, updatedBy);
            return Map(row);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Failed to update overtime application status");
            throw new AppException(ex.Message);
        }
    }

    public async Task DeleteApplicationAsync(int id, string deletedBy,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _sql.ExecuteAsync(
                "sp_OvertimeApplication",
                new { ActionType = "DELETE", Id = id, DeletedBy = deletedBy },
                cancellationToken);

            _logger.LogInformation("Overtime application {Id} deleted by {User}", id, deletedBy);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Failed to delete overtime application");
            throw new AppException(ex.Message);
        }
    }
}
