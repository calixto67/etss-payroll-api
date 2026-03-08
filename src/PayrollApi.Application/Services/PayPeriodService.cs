using Microsoft.Data.SqlClient;
using PayrollApi.Application.Common.Exceptions;
using PayrollApi.Application.DTOs.PayPeriod;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class PayPeriodService : IPayPeriodService
{
    private const string SP = "sp_PayPeriod";
    private readonly ISqlExecutor _sql;

    public PayPeriodService(ISqlExecutor sql) => _sql = sql;

    // ── Internal row class for Dapper mapping ────────────────────────

    private class PayPeriodRow
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string PeriodCode { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime PayDate { get; set; }
        public string PeriodType { get; set; } = "";
        public string Status { get; set; } = "";
        public bool IsClosed { get; set; }
        public int PayrollRecordCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // ── Status helpers ───────────────────────────────────────────────

    private static string? NormalizeStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status) || status == "all")
            return null;

        return status.Trim().ToLower() switch
        {
            "draft"    => "Draft",
            "open"     => "Open",
            "computed" => "Computed",
            "approved" => "Approved",
            "locked"   => "Locked",
            "exported" => "Exported",
            "1"        => "Draft",
            "2"        => "Open",
            "3"        => "Computed",
            "4"        => "Approved",
            "5"        => "Locked",
            "6"        => "Exported",
            _ => null
        };
    }

    private static PayPeriodDto MapRow(PayPeriodRow r) => new()
    {
        Id            = r.Id,
        Name          = r.Name,
        PeriodCode    = r.PeriodCode,
        StartDate     = r.StartDate,
        EndDate       = r.EndDate,
        PayDate       = r.PayDate,
        PeriodType    = r.PeriodType,
        Status        = r.Status?.ToLower() ?? "",
        IsClosed      = r.IsClosed,
        EmployeeCount = r.PayrollRecordCount,
        CreatedAt     = r.CreatedAt,
    };

    // ── Service methods ──────────────────────────────────────────────

    public async Task<IEnumerable<PayPeriodDto>> GetAllAsync(int? year, string? status, CancellationToken ct)
    {
        try
        {
            var rows = await _sql.QueryAsync<PayPeriodRow>(
                SP,
                new { ActionType = "GET_ALL", Year = year, Status = NormalizeStatus(status) },
                ct);

            return rows.Select(MapRow);
        }
        catch (SqlException ex)
        {
            throw new AppException($"Failed to retrieve pay periods: {ex.Message}");
        }
    }

    public async Task<PayPeriodDto> GetByIdAsync(int id, CancellationToken ct)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<PayPeriodRow>(
                SP,
                new { ActionType = "GET_BY_ID", Id = id },
                ct)
                ?? throw new NotFoundException("PayPeriod", id);

            return MapRow(row);
        }
        catch (SqlException ex)
        {
            throw new AppException($"Failed to retrieve pay period: {ex.Message}");
        }
    }

    public async Task<PayPeriodDto> CreateAsync(CreatePayPeriodDto dto, string createdBy, CancellationToken ct)
    {
        if (dto.EndDate <= dto.StartDate)
            throw new AppException("End date must be after start date.");
        if (dto.PayDate < dto.EndDate)
            throw new AppException("Pay date must be on or after end date.");

        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<PayPeriodRow>(
                SP,
                new
                {
                    ActionType = "CREATE",
                    StartDate  = dto.StartDate,
                    EndDate    = dto.EndDate,
                    PayDate    = dto.PayDate,
                    PeriodType = (int)dto.PeriodType,
                    CreatedBy  = createdBy,
                },
                ct)
                ?? throw new AppException("Failed to create pay period.");

            return MapRow(row);
        }
        catch (SqlException ex)
        {
            throw new AppException($"Failed to create pay period: {ex.Message}");
        }
    }

    public async Task<PayPeriodDto> UpdateStatusAsync(int id, string status, string updatedBy, CancellationToken ct)
    {
        var statusStr = NormalizeStatus(status)
            ?? throw new AppException($"Invalid status '{status}'.");

        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<PayPeriodRow>(
                SP,
                new
                {
                    ActionType = "UPDATE_STATUS",
                    Id         = id,
                    Status     = statusStr,
                    UpdatedBy  = updatedBy,
                },
                ct)
                ?? throw new NotFoundException("PayPeriod", id);

            return MapRow(row);
        }
        catch (SqlException ex)
        {
            throw new AppException($"Failed to update pay period status: {ex.Message}");
        }
    }

    public async Task DeleteAsync(int id, string deletedBy, CancellationToken ct)
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
            throw new AppException($"Failed to delete pay period: {ex.Message}");
        }
    }
}
