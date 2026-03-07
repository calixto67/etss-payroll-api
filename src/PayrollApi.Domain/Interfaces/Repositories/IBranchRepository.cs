using PayrollApi.Domain.Entities;

namespace PayrollApi.Domain.Interfaces.Repositories;

public interface IBranchRepository : IBaseRepository<Branch>
{
    Task<Branch?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
