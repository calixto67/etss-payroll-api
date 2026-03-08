using PayrollApi.Application.DTOs.ContributionBracket;

namespace PayrollApi.Application.Services.Interfaces;

public interface IContributionBracketService
{
    Task<IEnumerable<ContributionBracketDto>> GetAllAsync(string contributionType, CancellationToken ct = default);
    Task<ContributionBracketDto> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ContributionBracketDto> CreateAsync(CreateContributionBracketDto dto, string createdBy, CancellationToken ct = default);
    Task<ContributionBracketDto> UpdateAsync(int id, UpdateContributionBracketDto dto, string updatedBy, CancellationToken ct = default);
    Task DeleteAsync(int id, string deletedBy, CancellationToken ct = default);
}
