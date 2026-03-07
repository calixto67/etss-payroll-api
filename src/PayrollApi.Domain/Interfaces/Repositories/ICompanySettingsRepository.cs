using PayrollApi.Domain.Entities;

namespace PayrollApi.Domain.Interfaces.Repositories;

public interface ICompanySettingsRepository : IBaseRepository<CompanySettings>
{
    Task<CompanySettings?> GetSingletonAsync(CancellationToken cancellationToken = default);
}
