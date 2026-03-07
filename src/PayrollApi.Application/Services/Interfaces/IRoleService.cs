using PayrollApi.Application.DTOs.Role;

namespace PayrollApi.Application.Services.Interfaces;

public interface IRoleService
{
    Task<IEnumerable<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RoleWithPermissionsDto> GetWithPermissionsAsync(int id, CancellationToken cancellationToken = default);
    Task<RoleDto> CreateAsync(CreateRoleDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task<RoleDto> UpdateAsync(int id, UpdateRoleDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string deletedBy, CancellationToken cancellationToken = default);
    Task<RoleWithPermissionsDto> UpdatePermissionsAsync(int id, UpdatePermissionsDto dto, string updatedBy, CancellationToken cancellationToken = default);

    /// <summary>Returns all active users for the "Add user to role" picker.</summary>
    Task<IEnumerable<RoleUserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns users currently assigned to the given role.</summary>
    Task<IEnumerable<RoleUserDto>> GetUsersForRoleAsync(int roleId, CancellationToken cancellationToken = default);

    /// <summary>Assigns a user to a role (replaces any previous role assignment).</summary>
    Task AssignUserAsync(int roleId, int userId, string assignedBy, CancellationToken cancellationToken = default);

    /// <summary>Removes a user's role assignment.</summary>
    Task RemoveUserAsync(int roleId, int userId, string removedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the effective module permissions for a user.
    /// Admins receive all-true permissions regardless of role assignment.
    /// </summary>
    Task<Dictionary<string, ModulePermissionDto>> GetUserPermissionsAsync(int userId, string userRole, CancellationToken cancellationToken = default);
}
