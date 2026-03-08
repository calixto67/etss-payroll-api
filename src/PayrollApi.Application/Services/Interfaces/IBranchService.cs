using PayrollApi.Application.DTOs.Branch;

namespace PayrollApi.Application.Services.Interfaces;

public interface IBranchService
{
    Task<IEnumerable<BranchDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<BranchDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<BranchDto> CreateAsync(CreateBranchDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task<BranchDto> UpdateAsync(int id, UpdateBranchDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string deletedBy, CancellationToken cancellationToken = default);
}
