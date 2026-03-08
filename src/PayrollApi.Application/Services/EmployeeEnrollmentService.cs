using Microsoft.Data.SqlClient;
using PayrollApi.Application.Common.Exceptions;
using PayrollApi.Application.DTOs.EmployeeEnrollment;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class EmployeeEnrollmentService : IEmployeeEnrollmentService
{
    private const string SP = "sp_EmployeeEnrollment";

    private readonly ISqlExecutor _sql;

    public EmployeeEnrollmentService(ISqlExecutor sql) => _sql = sql;

    // ── Internal row types for Dapper mapping ──────────────────────────

    private sealed class AllowanceRow
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int AllowanceTypeId { get; set; }
        public decimal Amount { get; set; }
        public bool IsActive { get; set; }
        public string Frequency { get; set; } = "Monthly";
        public string? Remarks { get; set; }
        public string EmployeeCode { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string AllowanceTypeName { get; set; } = "";
        public bool IsDeMinimis { get; set; }
        public decimal TaxExemptLimit { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int? BranchId { get; set; }
        public string? BranchName { get; set; }
        public string? PositionTitle { get; set; }
    }

    private sealed class DeductionRow
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int DeductionTypeId { get; set; }
        public decimal Amount { get; set; }
        public bool IsActive { get; set; }
        public string Frequency { get; set; } = "Monthly";
        public string? Remarks { get; set; }
        public string EmployeeCode { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string DeductionTypeName { get; set; } = "";
        public bool IsMandatory { get; set; }
        public decimal DefaultAmount { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int? BranchId { get; set; }
        public string? BranchName { get; set; }
        public string? PositionTitle { get; set; }
    }

    // ── Allowances ─────────────────────────────────────────────────────

    public async Task<IEnumerable<EmployeeAllowanceDto>> GetAllowancesAsync(int? employeeId = null, string? search = null, int? departmentId = null, int? branchId = null, CancellationToken ct = default)
    {
        try
        {
            var rows = await _sql.QueryAsync<AllowanceRow>(SP, new { ActionType = "GET_ALLOWANCES", EmployeeId = employeeId, Search = search, DepartmentId = departmentId, BranchId = branchId }, ct);
            return rows.Select(ToAllowanceDto);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<EmployeeAllowanceDto> GetAllowanceByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<AllowanceRow>(
                SP, new { ActionType = "GET_ALLOWANCE_BY_ID", Id = id }, ct)
                ?? throw new AppException($"Employee allowance {id} not found.");
            return ToAllowanceDto(row);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<EmployeeAllowanceDto> CreateAllowanceAsync(CreateEmployeeAllowanceDto dto, string createdBy, CancellationToken ct = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<AllowanceRow>(SP, new
            {
                ActionType      = "CREATE_ALLOWANCE",
                EmployeeId      = dto.EmployeeId,
                AllowanceTypeId = dto.AllowanceTypeId,
                Amount          = dto.Amount,
                IsActive        = dto.IsActive,
                Frequency       = dto.Frequency,
                Remarks         = dto.Remarks,
                CreatedBy       = createdBy,
            }, ct) ?? throw new AppException("Failed to create employee allowance.");

            return ToAllowanceDto(row);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<EmployeeAllowanceDto> UpdateAllowanceAsync(int id, UpdateEmployeeAllowanceDto dto, string updatedBy, CancellationToken ct = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<AllowanceRow>(SP, new
            {
                ActionType    = "UPDATE_ALLOWANCE",
                Id            = id,
                Amount        = dto.Amount,
                IsActive      = dto.IsActive,
                Frequency     = dto.Frequency,
                Remarks       = dto.Remarks,
                UpdatedBy     = updatedBy,
            }, ct) ?? throw new AppException($"Employee allowance {id} not found.");

            return ToAllowanceDto(row);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task DeleteAllowanceAsync(int id, string deletedBy, CancellationToken ct = default)
    {
        try
        {
            await _sql.ExecuteAsync(SP, new { ActionType = "DELETE_ALLOWANCE", Id = id, DeletedBy = deletedBy }, ct);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    // ── Deductions ─────────────────────────────────────────────────────

    public async Task<IEnumerable<EmployeeDeductionDto>> GetDeductionsAsync(int? employeeId = null, string? search = null, int? departmentId = null, int? branchId = null, CancellationToken ct = default)
    {
        try
        {
            var rows = await _sql.QueryAsync<DeductionRow>(SP, new { ActionType = "GET_DEDUCTIONS", EmployeeId = employeeId, Search = search, DepartmentId = departmentId, BranchId = branchId }, ct);
            return rows.Select(ToDeductionDto);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<EmployeeDeductionDto> GetDeductionByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<DeductionRow>(
                SP, new { ActionType = "GET_DEDUCTION_BY_ID", Id = id }, ct)
                ?? throw new AppException($"Employee deduction {id} not found.");
            return ToDeductionDto(row);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<EmployeeDeductionDto> CreateDeductionAsync(CreateEmployeeDeductionDto dto, string createdBy, CancellationToken ct = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<DeductionRow>(SP, new
            {
                ActionType      = "CREATE_DEDUCTION",
                EmployeeId      = dto.EmployeeId,
                DeductionTypeId = dto.DeductionTypeId,
                Amount          = dto.Amount,
                IsActive        = dto.IsActive,
                Frequency       = dto.Frequency,
                Remarks         = dto.Remarks,
                CreatedBy       = createdBy,
            }, ct) ?? throw new AppException("Failed to create employee deduction.");

            return ToDeductionDto(row);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<EmployeeDeductionDto> UpdateDeductionAsync(int id, UpdateEmployeeDeductionDto dto, string updatedBy, CancellationToken ct = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<DeductionRow>(SP, new
            {
                ActionType    = "UPDATE_DEDUCTION",
                Id            = id,
                Amount        = dto.Amount,
                IsActive      = dto.IsActive,
                Frequency     = dto.Frequency,
                Remarks       = dto.Remarks,
                UpdatedBy     = updatedBy,
            }, ct) ?? throw new AppException($"Employee deduction {id} not found.");

            return ToDeductionDto(row);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task DeleteDeductionAsync(int id, string deletedBy, CancellationToken ct = default)
    {
        try
        {
            await _sql.ExecuteAsync(SP, new { ActionType = "DELETE_DEDUCTION", Id = id, DeletedBy = deletedBy }, ct);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    // ── Helpers ────────────────────────────────────────────────────────

    private static EmployeeAllowanceDto ToAllowanceDto(AllowanceRow r) => new()
    {
        Id                = r.Id,
        EmployeeId        = r.EmployeeId,
        AllowanceTypeId   = r.AllowanceTypeId,
        Amount            = r.Amount,
        IsActive          = r.IsActive,
        Frequency         = r.Frequency,
        Remarks           = r.Remarks,
        EmployeeCode      = r.EmployeeCode,
        EmployeeName      = r.EmployeeName,
        AllowanceTypeName = r.AllowanceTypeName,
        IsDeMinimis       = r.IsDeMinimis,
        TaxExemptLimit    = r.TaxExemptLimit,
        DepartmentId      = r.DepartmentId,
        DepartmentName    = r.DepartmentName,
        BranchId          = r.BranchId,
        BranchName        = r.BranchName,
        PositionTitle     = r.PositionTitle,
    };

    private static EmployeeDeductionDto ToDeductionDto(DeductionRow r) => new()
    {
        Id                = r.Id,
        EmployeeId        = r.EmployeeId,
        DeductionTypeId   = r.DeductionTypeId,
        Amount            = r.Amount,
        IsActive          = r.IsActive,
        Frequency         = r.Frequency,
        Remarks           = r.Remarks,
        EmployeeCode      = r.EmployeeCode,
        EmployeeName      = r.EmployeeName,
        DeductionTypeName = r.DeductionTypeName,
        IsMandatory       = r.IsMandatory,
        DefaultAmount     = r.DefaultAmount,
        DepartmentId      = r.DepartmentId,
        DepartmentName    = r.DepartmentName,
        BranchId          = r.BranchId,
        BranchName        = r.BranchName,
        PositionTitle     = r.PositionTitle,
    };
}
