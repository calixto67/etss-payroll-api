using Microsoft.EntityFrameworkCore;
using PayrollApi.Domain.Entities;
using PayrollApi.Domain.Interfaces.Repositories;
using PayrollApi.Infrastructure.Data;

namespace PayrollApi.Infrastructure.Repositories;

public class GlobalConfigRepository : IGlobalConfigRepository
{
    private readonly AppDbContext _context;
    private readonly DbSet<GlobalConfig> _dbSet;

    public GlobalConfigRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<GlobalConfig>();
    }

    public async Task<GlobalConfig?> GetByConfigNameAsync(string configName, CancellationToken cancellationToken = default) =>
        await _dbSet.AsNoTracking().FirstOrDefaultAsync(g => g.ConfigName == configName, cancellationToken);

    public async Task<GlobalConfig?> GetByConfigNameForUpdateAsync(string configName, CancellationToken cancellationToken = default) =>
        await _dbSet.FirstOrDefaultAsync(g => g.ConfigName == configName, cancellationToken);

    public async Task<GlobalConfig> AddAsync(GlobalConfig entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task UpdateAsync(GlobalConfig entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }
}
