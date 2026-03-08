using Microsoft.EntityFrameworkCore;
using PayrollApi.Domain.Entities;
using PayrollApi.Domain.Interfaces.Repositories;
using PayrollApi.Infrastructure.Data;

namespace PayrollApi.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await _dbSet.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        await _dbSet.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);

    public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default) =>
        !await _dbSet.AnyAsync(u =>
            u.Email == email && (!excludeId.HasValue || u.Id != excludeId.Value),
            cancellationToken);

    public async Task<bool> IsUsernameUniqueAsync(string username, int? excludeId = null, CancellationToken cancellationToken = default) =>
        !await _dbSet.AnyAsync(u =>
            u.Username == username && (!excludeId.HasValue || u.Id != excludeId.Value),
            cancellationToken);

    public async Task<IEnumerable<User>> GetAllActiveAsync(CancellationToken cancellationToken = default) =>
        await _dbSet
            .Where(u => u.IsActive && !u.IsDeleted)
            .Include(u => u.PermissionRole)
            .OrderBy(u => u.Username)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<User>> GetByRoleIdAsync(int roleId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .Where(u => u.RoleId == roleId && u.IsActive && !u.IsDeleted)
            .OrderBy(u => u.Username)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
}
