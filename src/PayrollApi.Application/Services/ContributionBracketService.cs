using Microsoft.Data.SqlClient;
using PayrollApi.Application.Common.Exceptions;
using PayrollApi.Application.DTOs.ContributionBracket;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class ContributionBracketService : IContributionBracketService
{
    private const string SP = "sp_ContributionBracket";

    private readonly ISqlExecutor _sql;

    public ContributionBracketService(ISqlExecutor sql) => _sql = sql;

    private sealed class ContributionBracketRow
    {
        public int Id { get; set; }
        public string ContributionType { get; set; } = "";
        public decimal RangeFrom { get; set; }
        public decimal? RangeTo { get; set; }
        public decimal EmployeeShare { get; set; }
        public decimal EmployerShare { get; set; }
    }

    public async Task<IEnumerable<ContributionBracketDto>> GetAllAsync(string contributionType, CancellationToken ct = default)
    {
        try
        {
            var rows = await _sql.QueryAsync<ContributionBracketRow>(
                SP, new { ActionType = "GET_ALL", ContributionType = contributionType }, ct);
            return rows.Select(ToDto);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<ContributionBracketDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<ContributionBracketRow>(
                SP, new { ActionType = "GET_BY_ID", Id = id }, ct)
                ?? throw new AppException($"Contribution bracket {id} not found.");
            return ToDto(row);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<ContributionBracketDto> CreateAsync(CreateContributionBracketDto dto, string createdBy, CancellationToken ct = default)
    {
        ValidateContributionBracket(dto.ContributionType, dto.RangeFrom, dto.RangeTo, dto.EmployeeShare, dto.EmployerShare);
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<ContributionBracketRow>(SP, new
            {
                ActionType       = "CREATE",
                ContributionType = dto.ContributionType.Trim(),
                RangeFrom        = dto.RangeFrom,
                RangeTo          = dto.RangeTo,
                EmployeeShare    = dto.EmployeeShare,
                EmployerShare    = dto.EmployerShare,
                CreatedBy        = createdBy,
            }, ct) ?? throw new AppException("Failed to create contribution bracket.");

            return ToDto(row);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<ContributionBracketDto> UpdateAsync(int id, UpdateContributionBracketDto dto, string updatedBy, CancellationToken ct = default)
    {
        ValidateContributionBracket(null, dto.RangeFrom, dto.RangeTo, dto.EmployeeShare, dto.EmployerShare);
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<ContributionBracketRow>(SP, new
            {
                ActionType    = "UPDATE",
                Id            = id,
                RangeFrom     = dto.RangeFrom,
                RangeTo       = dto.RangeTo,
                EmployeeShare = dto.EmployeeShare,
                EmployerShare = dto.EmployerShare,
                UpdatedBy     = updatedBy,
            }, ct) ?? throw new AppException($"Contribution bracket {id} not found.");

            return ToDto(row);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    private static void ValidateContributionBracket(string? contributionType, decimal rangeFrom, decimal? rangeTo, decimal employeeShare, decimal employerShare)
    {
        if (contributionType is not null && string.IsNullOrWhiteSpace(contributionType))
            throw new AppException("Contribution type is required.");
        if (contributionType is not null && contributionType.Trim() is not ("SSS" or "PhilHealth" or "PagIBIG"))
            throw new AppException("Contribution type must be SSS, PhilHealth, or PagIBIG.");
        if (rangeFrom < 0)
            throw new AppException("Salary Range From must be 0 or greater.");
        if (rangeTo.HasValue && rangeTo.Value <= rangeFrom)
            throw new AppException("Salary Range To must be greater than Range From.");
        if (employeeShare < 0)
            throw new AppException("Employee Share must be 0 or greater.");
        if (employerShare < 0)
            throw new AppException("Employer Share must be 0 or greater.");
        if (employeeShare == 0 && employerShare == 0)
            throw new AppException("At least one share (Employee or Employer) must be greater than 0.");
    }

    public async Task DeleteAsync(int id, string deletedBy, CancellationToken ct = default)
    {
        try
        {
            await _sql.ExecuteAsync(SP, new
            {
                ActionType = "DELETE",
                Id         = id,
                UpdatedBy  = deletedBy,
            }, ct);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    private static ContributionBracketDto ToDto(ContributionBracketRow r) => new()
    {
        Id               = r.Id,
        ContributionType = r.ContributionType,
        RangeFrom        = r.RangeFrom,
        RangeTo          = r.RangeTo,
        EmployeeShare    = r.EmployeeShare,
        EmployerShare    = r.EmployerShare,
    };
}
