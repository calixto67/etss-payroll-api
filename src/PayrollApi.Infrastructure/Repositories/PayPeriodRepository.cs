using Microsoft.EntityFrameworkCore;
using PayrollApi.Domain.Entities;
using PayrollApi.Domain.Interfaces.Repositories;
using PayrollApi.Infrastructure.Data;

namespace PayrollApi.Infrastructure.Repositories;

public class PayPeriodRepository : BaseRepository<PayrollPeriod>, IPayPeriodRepository
{
    public PayPeriodRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<PayrollPeriod>> GetFilteredAsync(
        int? year, PeriodStatus? status,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(p => p.PayrollRecords)
            .AsNoTracking()
            .AsQueryable();

        if (year.HasValue)
            query = query.Where(p => p.StartDate.Year == year.Value);

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        return await query
            .OrderByDescending(p => p.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> PeriodCodeExistsAsync(string code, CancellationToken cancellationToken = default) =>
        await _dbSet.AnyAsync(p => p.PeriodCode == code, cancellationToken);

    public async Task<string?> GetLastAlphabetPrefixAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .OrderByDescending(p => p.Name)
            .Select(p => p.Name.Substring(0, 2))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
