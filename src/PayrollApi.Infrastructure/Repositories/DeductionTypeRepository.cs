using Microsoft.EntityFrameworkCore;
using PayrollApi.Domain.Entities;
using PayrollApi.Domain.Interfaces.Repositories;
using PayrollApi.Infrastructure.Data;

namespace PayrollApi.Infrastructure.Repositories;

public class DeductionTypeRepository : BaseRepository<DeductionType>, IDeductionTypeRepository
{
    public DeductionTypeRepository(AppDbContext context) : base(context) { }

    public async Task<bool> IsNameUniqueAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default) =>
        !await _dbSet.AnyAsync(a =>
            a.Name == name && (!excludeId.HasValue || a.Id != excludeId.Value),
            cancellationToken);
}
