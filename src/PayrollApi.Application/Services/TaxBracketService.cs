using Microsoft.Data.SqlClient;
using PayrollApi.Application.Common.Exceptions;
using PayrollApi.Application.DTOs.TaxBracket;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class TaxBracketService : ITaxBracketService
{
    private const string SP = "sp_TaxBracket";

    private readonly ISqlExecutor _sql;

    public TaxBracketService(ISqlExecutor sql) => _sql = sql;

    private sealed class TaxBracketRow
    {
        public int Id { get; set; }
        public string BracketName { get; set; } = "";
        public decimal MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public decimal BaseTax { get; set; }
        public decimal TaxRate { get; set; }
        public decimal ExcessOver { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsActive { get; set; }
    }

    public async Task<IEnumerable<TaxBracketDto>> GetAllAsync(CancellationToken ct = default)
    {
        try
        {
            var rows = await _sql.QueryAsync<TaxBracketRow>(SP, new { ActionType = "GET_ALL" }, ct);
            return rows.Select(ToDto);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<TaxBracketDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<TaxBracketRow>(
                SP, new { ActionType = "GET_BY_ID", Id = id }, ct)
                ?? throw new AppException($"Tax bracket {id} not found.");
            return ToDto(row);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<TaxBracketDto> CreateAsync(CreateTaxBracketDto dto, string createdBy, CancellationToken ct = default)
    {
        ValidateTaxBracket(dto.BracketName, dto.MinAmount, dto.MaxAmount, dto.BaseTax, dto.TaxRate, dto.ExcessOver);
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<TaxBracketRow>(SP, new
            {
                ActionType  = "CREATE",
                BracketName = dto.BracketName.Trim(),
                MinAmount   = dto.MinAmount,
                MaxAmount   = dto.MaxAmount,
                BaseTax     = dto.BaseTax,
                TaxRate     = dto.TaxRate,
                ExcessOver  = dto.ExcessOver,
                CreatedBy   = createdBy,
            }, ct) ?? throw new AppException("Failed to create tax bracket.");

            return ToDto(row);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<TaxBracketDto> UpdateAsync(int id, UpdateTaxBracketDto dto, string updatedBy, CancellationToken ct = default)
    {
        ValidateTaxBracket(dto.BracketName, dto.MinAmount, dto.MaxAmount, dto.BaseTax, dto.TaxRate, dto.ExcessOver);
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<TaxBracketRow>(SP, new
            {
                ActionType  = "UPDATE",
                Id          = id,
                BracketName = dto.BracketName.Trim(),
                MinAmount   = dto.MinAmount,
                MaxAmount   = dto.MaxAmount,
                BaseTax     = dto.BaseTax,
                TaxRate     = dto.TaxRate,
                ExcessOver  = dto.ExcessOver,
                UpdatedBy   = updatedBy,
            }, ct) ?? throw new AppException($"Tax bracket {id} not found.");

            return ToDto(row);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    private static void ValidateTaxBracket(string bracketName, decimal minAmount, decimal? maxAmount, decimal baseTax, decimal taxRate, decimal excessOver)
    {
        if (string.IsNullOrWhiteSpace(bracketName))
            throw new AppException("Bracket name is required.");
        if (minAmount < 0)
            throw new AppException("Min Amount must be 0 or greater.");
        if (maxAmount.HasValue && maxAmount.Value <= minAmount)
            throw new AppException("Max Amount must be greater than Min Amount.");
        if (baseTax < 0)
            throw new AppException("Base Tax must be 0 or greater.");
        if (taxRate < 0 || taxRate > 1)
            throw new AppException("Tax Rate must be between 0 and 1.");
        if (excessOver < 0)
            throw new AppException("Excess Over must be 0 or greater.");
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

    private static TaxBracketDto ToDto(TaxBracketRow r) => new()
    {
        Id            = r.Id,
        BracketName   = r.BracketName,
        MinAmount     = r.MinAmount,
        MaxAmount     = r.MaxAmount,
        BaseTax       = r.BaseTax,
        TaxRate       = r.TaxRate,
        ExcessOver    = r.ExcessOver,
        EffectiveDate = r.EffectiveDate.ToString("yyyy-MM-dd"),
        IsActive      = r.IsActive,
    };
}
