using Microsoft.EntityFrameworkCore;
using PayrollApi.Domain.Entities;
using PayrollApi.Domain.Interfaces.Repositories;
using PayrollApi.Infrastructure.Data;

namespace PayrollApi.Infrastructure.Repositories;

public class RoleRepository : BaseRepository<Role>, IRoleRepository
{
    public RoleRepository(AppDbContext context) : base(context) { }

    public async Task<Role?> GetWithPermissionsAsync(int id, CancellationToken ct = default) =>
        await _dbSet
            .Include(r => r.Permissions)
            .Include(r => r.Users)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, ct);

    public async Task<IEnumerable<Role>> GetAllWithPermissionsAsync(CancellationToken ct = default) =>
        await _dbSet
            .Include(r => r.Permissions)
            .Include(r => r.Users)
            .Where(r => !r.IsDeleted)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<bool> IsNameUniqueAsync(string name, int? excludeId = null, CancellationToken ct = default) =>
        !await _dbSet.AnyAsync(r =>
            r.Name == name &&
            !r.IsDeleted &&
            (excludeId == null || r.Id != excludeId), ct);

    public async Task<IEnumerable<RolePermission>> GetPermissionsForRoleAsync(int roleId, CancellationToken ct = default) =>
        await _context.Set<RolePermission>()
            .Where(p => p.RoleId == roleId && !p.IsDeleted)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task ReplacePermissionsAsync(int roleId, IEnumerable<RolePermission> permissions, CancellationToken ct = default)
    {
        var existing = await _context.Set<RolePermission>()
            .Where(p => p.RoleId == roleId)
            .ToListAsync(ct);

        _context.Set<RolePermission>().RemoveRange(existing);

        await _context.Set<RolePermission>().AddRangeAsync(permissions, ct);
    }
}
