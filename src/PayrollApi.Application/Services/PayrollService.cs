using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using PayrollApi.Application.Common.Exceptions;
using PayrollApi.Application.Common.Models;
using PayrollApi.Application.DTOs.Payroll;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class PayrollService : IPayrollService
{
    private readonly ISqlExecutor _sql;
    private readonly ILogger<PayrollService> _logger;

    public PayrollService(ISqlExecutor sql, ILogger<PayrollService> logger)
    {
        _sql = sql;
        _logger = logger;
    }

    public async Task<PagedResult<PayrollRecordDto>> GetPagedAsync(
        PaginationParams pagination, int? employeeId, int? periodId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (rows, totalCount) = await _sql.QueryPagedAsync<PayrollRecordRow>(
                "sp_Payroll",
                new { ActionType = "GET_PAGED", Page = pagination.Page, PageSize = pagination.PageSize, EmployeeId = employeeId, PeriodId = periodId },
                cancellationToken);

            var dtos = rows.Select(MapToDto);
            return PagedResult<PayrollRecordDto>.Create(dtos, totalCount, pagination.Page, pagination.PageSize);
        }
        catch (SqlException ex) { _logger.LogError(ex, "SQL error in GetPagedAsync"); throw new AppException(ex.Message); }
    }

    public async Task<PayrollRecordDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<PayrollRecordRow>(
                "sp_Payroll", new { ActionType = "GET_BY_ID", Id = id }, cancellationToken)
                ?? throw new NotFoundException("PayrollRecord", id);
            return MapToDto(row);
        }
        catch (SqlException ex) { _logger.LogError(ex, "SQL error in GetByIdAsync {Id}", id); throw new AppException(ex.Message); }
    }

    public async Task<IEnumerable<PayrollRecordDto>> RunPayrollAsync(
        RunPayrollDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Running payroll for period {PeriodId} by {By}", dto.PayrollPeriodId, dto.InitiatedBy);
        try
        {
            var employeeIdsCsv = dto.EmployeeIds?.Any() == true ? string.Join(",", dto.EmployeeIds) : null;

            var rows = await _sql.QueryAsync<PayrollRecordRow>(
                "sp_Payroll",
                new { ActionType = "RUN", PeriodId = dto.PayrollPeriodId, EmployeeIds = employeeIdsCsv, InitiatedBy = dto.InitiatedBy },
                cancellationToken);

            var dtos = rows.Select(MapToDto).ToList();
            _logger.LogInformation("Payroll run complete. {Count} records created.", dtos.Count);
            return dtos;
        }
        catch (SqlException ex) { _logger.LogError(ex, "SQL error in RunPayrollAsync"); throw new AppException(ex.Message); }
    }

    public async Task<PayrollRecordDto> ApproveAsync(int id, string approvedBy, CancellationToken cancellationToken = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<PayrollRecordRow>(
                "sp_Payroll", new { ActionType = "APPROVE", Id = id, ApprovedBy = approvedBy }, cancellationToken)
                ?? throw new NotFoundException("PayrollRecord", id);
            _logger.LogInformation("Payroll record {Id} approved by {By}", id, approvedBy);
            return MapToDto(row);
        }
        catch (SqlException ex) { _logger.LogError(ex, "SQL error in ApproveAsync {Id}", id); throw new AppException(ex.Message); }
    }

    public async Task<PayrollRecordDto> ReleaseAsync(int id, string releasedBy, CancellationToken cancellationToken = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<PayrollRecordRow>(
                "sp_Payroll", new { ActionType = "RELEASE", Id = id, ReleasedBy = releasedBy }, cancellationToken)
                ?? throw new NotFoundException("PayrollRecord", id);
            _logger.LogInformation("Payroll record {Id} released by {By}", id, releasedBy);
            return MapToDto(row);
        }
        catch (SqlException ex) { _logger.LogError(ex, "SQL error in ReleaseAsync {Id}", id); throw new AppException(ex.Message); }
    }

    public async Task<IEnumerable<PayrollRecordDto>> GetByPeriodAsync(int periodId, CancellationToken cancellationToken = default)
    {
        try
        {
            var rows = await _sql.QueryAsync<PayrollRecordRow>(
                "sp_Payroll", new { ActionType = "GET_BY_PERIOD", PeriodId = periodId }, cancellationToken);
            return rows.Select(MapToDto);
        }
        catch (SqlException ex) { _logger.LogError(ex, "SQL error in GetByPeriodAsync"); throw new AppException(ex.Message); }
    }

    private static PayrollRecordDto MapToDto(PayrollRecordRow r) => new()
    {
        Id = r.Id, EmployeeId = r.EmployeeId, PayrollPeriodId = r.PayrollPeriodId,
        EmployeeName = r.EmployeeName, EmployeeCode = r.EmployeeCode,
        PeriodCode = r.PeriodCode, PeriodName = r.PeriodName,
        BasicPay = r.BasicPay, OvertimePay = r.OvertimePay, HolidayPay = r.HolidayPay,
        Allowances = r.Allowances, GrossPay = r.GrossPay,
        SssDeduction = r.SssDeduction, PhilHealthDeduction = r.PhilHealthDeduction,
        PagIbigDeduction = r.PagIbigDeduction, TaxWithheld = r.TaxWithheld,
        OtherDeductions = r.OtherDeductions, TotalDeductions = r.TotalDeductions,
        NetPay = r.NetPay, Status = r.StatusName ?? $"Unknown({r.Status})",
        ProcessedAt = r.ProcessedAt, ProcessedBy = r.ProcessedBy, CreatedAt = r.CreatedAt
    };

    private sealed class PayrollRecordRow
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int PayrollPeriodId { get; set; }
        public string EmployeeName { get; set; } = "";
        public string EmployeeCode { get; set; } = "";
        public string PeriodCode { get; set; } = "";
        public string PeriodName { get; set; } = "";
        public decimal BasicPay { get; set; }
        public decimal OvertimePay { get; set; }
        public decimal HolidayPay { get; set; }
        public decimal Allowances { get; set; }
        public decimal GrossPay { get; set; }
        public decimal SssDeduction { get; set; }
        public decimal PhilHealthDeduction { get; set; }
        public decimal PagIbigDeduction { get; set; }
        public decimal TaxWithheld { get; set; }
        public decimal OtherDeductions { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NetPay { get; set; }
        public int Status { get; set; }
        public string? StatusName { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? ProcessedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
