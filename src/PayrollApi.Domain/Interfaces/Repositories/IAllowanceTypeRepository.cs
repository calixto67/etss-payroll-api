using PayrollApi.Domain.Entities;

namespace PayrollApi.Domain.Interfaces.Repositories;

public interface IAllowanceTypeRepository : IBaseRepository<AllowanceType>
{
    Task<bool> IsNameUniqueAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
}
