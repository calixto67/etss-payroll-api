using Microsoft.EntityFrameworkCore;
using PayrollApi.Domain.Entities;
using PayrollApi.Domain.Interfaces.Repositories;
using PayrollApi.Infrastructure.Data;

namespace PayrollApi.Infrastructure.Repositories;

public class BranchRepository : BaseRepository<Branch>, IBranchRepository
{
    public BranchRepository(AppDbContext context) : base(context) { }

    public async Task<Branch?> GetByCodeAsync(string code, CancellationToken cancellationToken = default) =>
        await _dbSet.FirstOrDefaultAsync(b => b.BranchCode == code, cancellationToken);
}
