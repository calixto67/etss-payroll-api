using Microsoft.Data.SqlClient;
using PayrollApi.Application.Common.Exceptions;
using PayrollApi.Application.DTOs.Department;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class DepartmentService : IDepartmentService
{
    private const string SP = "sp_Department";

    private readonly ISqlExecutor _sql;

    public DepartmentService(ISqlExecutor sql) => _sql = sql;

    private sealed class DepartmentRow
    {
        public int Id { get; set; }
        public string DepartmentCode { get; set; } = "";
        public string DepartmentName { get; set; } = "";
        public string? Description { get; set; }
        public int? ManagerId { get; set; }
    }

    public async Task<IEnumerable<DepartmentDto>> GetAllAsync(CancellationToken ct = default)
    {
        try
        {
            var rows = await _sql.QueryAsync<DepartmentRow>(SP, new { ActionType = "GET_ALL" }, ct);
            return rows.Select(ToDto);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<DepartmentDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<DepartmentRow>(
                SP, new { ActionType = "GET_BY_ID", Id = id }, ct)
                ?? throw new AppException($"Department {id} not found.");
            return ToDto(row);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto, string createdBy, CancellationToken ct = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<DepartmentRow>(SP, new
            {
                ActionType     = "CREATE",
                DepartmentCode = dto.DepartmentCode.Trim(),
                DepartmentName = dto.DepartmentName.Trim(),
                Description    = dto.Description?.Trim(),
                ManagerId      = dto.ManagerId,
                CreatedBy      = createdBy,
            }, ct) ?? throw new AppException("Failed to create department.");

            return ToDto(row);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<DepartmentDto> UpdateAsync(int id, UpdateDepartmentDto dto, string updatedBy, CancellationToken ct = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<DepartmentRow>(SP, new
            {
                ActionType     = "UPDATE",
                Id             = id,
                DepartmentCode = dto.DepartmentCode.Trim(),
                DepartmentName = dto.DepartmentName.Trim(),
                Description    = dto.Description?.Trim(),
                ManagerId      = dto.ManagerId,
                UpdatedBy      = updatedBy,
            }, ct) ?? throw new AppException($"Department {id} not found.");

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

    private static DepartmentDto ToDto(DepartmentRow r) => new()
    {
        Id             = r.Id,
        DepartmentCode = r.DepartmentCode,
        DepartmentName = r.DepartmentName,
        Description    = r.Description,
        ManagerId      = r.ManagerId,
    };
}
