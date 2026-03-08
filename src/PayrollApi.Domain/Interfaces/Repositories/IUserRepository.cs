using PayrollApi.Domain.Entities;

namespace PayrollApi.Domain.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> IsUsernameUniqueAsync(string username, int? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>All active users, optionally including their PermissionRole nav property.</summary>
    Task<IEnumerable<User>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>Users currently assigned to the given permission role.</summary>
    Task<IEnumerable<User>> GetByRoleIdAsync(int roleId, CancellationToken cancellationToken = default);
}
