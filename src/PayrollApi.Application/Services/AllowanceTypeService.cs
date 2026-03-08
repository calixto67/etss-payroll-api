using Microsoft.Data.SqlClient;
using PayrollApi.Application.Common.Exceptions;
using PayrollApi.Application.DTOs.AllowanceType;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class AllowanceTypeService : IAllowanceTypeService
{
    private const string SP = "sp_AllowanceType";

    private readonly ISqlExecutor _sql;

    public AllowanceTypeService(ISqlExecutor sql) => _sql = sql;

    // ── Internal row type for Dapper mapping ─────────────────────────────

    private sealed class AllowanceTypeRow
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public bool IsDeMinimis { get; set; }
        public decimal TaxExemptLimit { get; set; }
    }

    // ── Public methods ───────────────────────────────────────────────────

    public async Task<IEnumerable<AllowanceTypeDto>> GetAllAsync(CancellationToken ct = default)
    {
        try
        {
            var rows = await _sql.QueryAsync<AllowanceTypeRow>(SP, new { ActionType = "GET_ALL" }, ct);
            return rows.Select(ToDto);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<AllowanceTypeDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<AllowanceTypeRow>(
                SP, new { ActionType = "GET_BY_ID", Id = id }, ct)
                ?? throw new AppException($"Allowance type {id} not found.");
            return ToDto(row);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<AllowanceTypeDto> CreateAsync(CreateAllowanceTypeDto dto, string createdBy, CancellationToken ct = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<AllowanceTypeRow>(SP, new
            {
                ActionType     = "CREATE",
                Name           = dto.Name.Trim(),
                IsDeMinimis    = dto.IsDeMinimis,
                TaxExemptLimit = dto.TaxExemptLimit,
                CreatedBy      = createdBy,
            }, ct) ?? throw new AppException("Failed to create allowance type.");

            return ToDto(row);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<AllowanceTypeDto> UpdateAsync(int id, UpdateAllowanceTypeDto dto, string updatedBy, CancellationToken ct = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<AllowanceTypeRow>(SP, new
            {
                ActionType     = "UPDATE",
                Id             = id,
                Name           = dto.Name.Trim(),
                IsDeMinimis    = dto.IsDeMinimis,
                TaxExemptLimit = dto.TaxExemptLimit,
                UpdatedBy      = updatedBy,
            }, ct) ?? throw new AppException($"Allowance type {id} not found.");

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

    private static AllowanceTypeDto ToDto(AllowanceTypeRow r) => new()
    {
        Id             = r.Id,
        Name           = r.Name,
        IsDeMinimis    = r.IsDeMinimis,
        TaxExemptLimit = r.TaxExemptLimit,
    };
}
