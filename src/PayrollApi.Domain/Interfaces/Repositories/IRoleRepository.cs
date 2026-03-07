using PayrollApi.Domain.Entities;

namespace PayrollApi.Domain.Interfaces.Repositories;

public interface IRoleRepository : IBaseRepository<Role>
{
    Task<Role?> GetWithPermissionsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Role>> GetAllWithPermissionsAsync(CancellationToken cancellationToken = default);
    Task<bool> IsNameUniqueAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);

    // RolePermission operations
    Task<IEnumerable<RolePermission>> GetPermissionsForRoleAsync(int roleId, CancellationToken cancellationToken = default);
    Task ReplacePermissionsAsync(int roleId, IEnumerable<RolePermission> permissions, CancellationToken cancellationToken = default);
}
