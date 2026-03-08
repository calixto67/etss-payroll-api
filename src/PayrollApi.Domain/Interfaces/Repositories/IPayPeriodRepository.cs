using PayrollApi.Domain.Entities;

namespace PayrollApi.Domain.Interfaces.Repositories;

public interface IPayPeriodRepository : IBaseRepository<PayrollPeriod>
{
    Task<IEnumerable<PayrollPeriod>> GetFilteredAsync(
        int? year, PeriodStatus? status,
        CancellationToken cancellationToken = default);

    Task<bool> PeriodCodeExistsAsync(string code, CancellationToken cancellationToken = default);

    Task<string?> GetLastAlphabetPrefixAsync(CancellationToken cancellationToken = default);
}
