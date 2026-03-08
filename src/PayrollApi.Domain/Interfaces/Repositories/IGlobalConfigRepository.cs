using PayrollApi.Domain.Entities;

namespace PayrollApi.Domain.Interfaces.Repositories;

public interface IGlobalConfigRepository
{
    Task<GlobalConfig?> GetByConfigNameAsync(string configName, CancellationToken cancellationToken = default);
    Task<GlobalConfig?> GetByConfigNameForUpdateAsync(string configName, CancellationToken cancellationToken = default);
    Task<GlobalConfig> AddAsync(GlobalConfig entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(GlobalConfig entity, CancellationToken cancellationToken = default);
}
