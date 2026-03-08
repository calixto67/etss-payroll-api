using PayrollApi.Application.DTOs.AllowanceType;

namespace PayrollApi.Application.Services.Interfaces;

public interface IAllowanceTypeService
{
    Task<IEnumerable<AllowanceTypeDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AllowanceTypeDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AllowanceTypeDto> CreateAsync(CreateAllowanceTypeDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task<AllowanceTypeDto> UpdateAsync(int id, UpdateAllowanceTypeDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string deletedBy, CancellationToken cancellationToken = default);
}
