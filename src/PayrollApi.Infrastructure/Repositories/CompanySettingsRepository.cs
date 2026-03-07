using Microsoft.EntityFrameworkCore;
using PayrollApi.Domain.Entities;
using PayrollApi.Domain.Interfaces.Repositories;
using PayrollApi.Infrastructure.Data;

namespace PayrollApi.Infrastructure.Repositories;

public class CompanySettingsRepository : BaseRepository<CompanySettings>, ICompanySettingsRepository
{
    public CompanySettingsRepository(AppDbContext context) : base(context) { }

    public async Task<CompanySettings?> GetSingletonAsync(CancellationToken ct = default) =>
        await _dbSet.FirstOrDefaultAsync(s => !s.IsDeleted, ct);
}
