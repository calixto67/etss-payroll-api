using PayrollApi.Application.DTOs.TaxBracket;

namespace PayrollApi.Application.Services.Interfaces;

public interface ITaxBracketService
{
    Task<IEnumerable<TaxBracketDto>> GetAllAsync(CancellationToken ct = default);
    Task<TaxBracketDto> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TaxBracketDto> CreateAsync(CreateTaxBracketDto dto, string createdBy, CancellationToken ct = default);
    Task<TaxBracketDto> UpdateAsync(int id, UpdateTaxBracketDto dto, string updatedBy, CancellationToken ct = default);
    Task DeleteAsync(int id, string deletedBy, CancellationToken ct = default);
}
