using PayrollApi.Application.DTOs.DeductionType;

namespace PayrollApi.Application.Services.Interfaces;

public interface IDeductionTypeService
{
    Task<IEnumerable<DeductionTypeDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DeductionTypeDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<DeductionTypeDto> CreateAsync(CreateDeductionTypeDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task<DeductionTypeDto> UpdateAsync(int id, UpdateDeductionTypeDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string deletedBy, CancellationToken cancellationToken = default);
}
