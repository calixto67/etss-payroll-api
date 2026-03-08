using Microsoft.Data.SqlClient;
using PayrollApi.Application.Common.Exceptions;
using PayrollApi.Application.DTOs.Branch;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class BranchService : IBranchService
{
    private const string SP = "sp_Branch";

    private readonly ISqlExecutor _sql;

    public BranchService(ISqlExecutor sql) => _sql = sql;

    private sealed class BranchRow
    {
        public int Id { get; set; }
        public string BranchCode { get; set; } = "";
        public string BranchName { get; set; } = "";
        public string? Address { get; set; }
        public bool IsHeadOffice { get; set; }
    }

    public async Task<IEnumerable<BranchDto>> GetAllAsync(CancellationToken ct = default)
    {
        try
        {
            var rows = await _sql.QueryAsync<BranchRow>(SP, new { ActionType = "GET_ALL" }, ct);
            return rows.Select(ToDto);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<BranchDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<BranchRow>(
                SP, new { ActionType = "GET_BY_ID", Id = id }, ct)
                ?? throw new AppException($"Branch {id} not found.");
            return ToDto(row);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<BranchDto> CreateAsync(CreateBranchDto dto, string createdBy, CancellationToken ct = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<BranchRow>(SP, new
            {
                ActionType   = "CREATE",
                BranchCode   = dto.BranchCode.Trim(),
                BranchName   = dto.BranchName.Trim(),
                Address      = dto.Address?.Trim(),
                IsHeadOffice = dto.IsHeadOffice,
                CreatedBy    = createdBy,
            }, ct) ?? throw new AppException("Failed to create branch.");

            return ToDto(row);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<BranchDto> UpdateAsync(int id, UpdateBranchDto dto, string updatedBy, CancellationToken ct = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<BranchRow>(SP, new
            {
                ActionType   = "UPDATE",
                Id           = id,
                BranchCode   = dto.BranchCode.Trim(),
                BranchName   = dto.BranchName.Trim(),
                Address      = dto.Address?.Trim(),
                IsHeadOffice = dto.IsHeadOffice,
                UpdatedBy    = updatedBy,
            }, ct) ?? throw new AppException($"Branch {id} not found.");

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

    private static BranchDto ToDto(BranchRow r) => new()
    {
        Id           = r.Id,
        BranchCode   = r.BranchCode,
        BranchName   = r.BranchName,
        Address      = r.Address,
        IsHeadOffice = r.IsHeadOffice,
    };
}
