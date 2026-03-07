using PayrollApi.Application.Common.Exceptions;
using PayrollApi.Application.DTOs.Role;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Entities;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class RoleService : IRoleService
{
    /// <summary>
    /// Canonical module keys that map to Angular sidemenu items.
    /// New modules only need to be added here — no other server-side change required.
    /// </summary>
    public static readonly IReadOnlyList<string> AllModuleKeys = new[]
    {
        "dashboard", "pay-periods", "attendance", "payroll-run",
        "approvals", "payslips", "employees", "leave",
        "reports", "setup", "user-roles"
    };

    private static readonly HashSet<string> AdminRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "Admin", "PayrollAdmin"
    };

    private readonly IUnitOfWork _uow;

    public RoleService(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<RoleDto>> GetAllAsync(CancellationToken ct = default)
    {
        var roles = await _uow.Roles.GetAllWithPermissionsAsync(ct);
        return roles.Select(ToDto);
    }

    public async Task<RoleWithPermissionsDto> GetWithPermissionsAsync(int id, CancellationToken ct = default)
    {
        var role = await _uow.Roles.GetWithPermissionsAsync(id, ct)
            ?? throw new AppException($"Role {id} not found.");
        return ToDetailDto(role);
    }

    public async Task<RoleDto> CreateAsync(CreateRoleDto dto, string createdBy, CancellationToken ct = default)
    {
        if (!await _uow.Roles.IsNameUniqueAsync(dto.Name, cancellationToken: ct))
            throw new AppException($"A role named '{dto.Name}' already exists.");

        var role = new Role
        {
            Name        = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            CreatedBy   = createdBy,
        };

        await _uow.Roles.AddAsync(role, ct);
        await _uow.CommitAsync(ct);
        return ToDto(role);
    }

    public async Task<RoleDto> UpdateAsync(int id, UpdateRoleDto dto, string updatedBy, CancellationToken ct = default)
    {
        var role = await _uow.Roles.GetByIdAsync(id, ct)
            ?? throw new AppException($"Role {id} not found.");

        if (!await _uow.Roles.IsNameUniqueAsync(dto.Name, excludeId: id, cancellationToken: ct))
            throw new AppException($"A role named '{dto.Name}' already exists.");

        role.Name        = dto.Name.Trim();
        role.Description = dto.Description?.Trim();
        role.UpdatedAt   = DateTime.UtcNow;
        role.UpdatedBy   = updatedBy;

        await _uow.Roles.UpdateAsync(role, ct);
        await _uow.CommitAsync(ct);
        return ToDto(role);
    }

    public async Task DeleteAsync(int id, string deletedBy, CancellationToken ct = default)
    {
        var role = await _uow.Roles.GetByIdAsync(id, ct)
            ?? throw new AppException($"Role {id} not found.");

        _ = role; // existence confirmed
        await _uow.Roles.DeleteAsync(id, deletedBy, ct);
        await _uow.CommitAsync(ct);
    }

    public async Task<RoleWithPermissionsDto> UpdatePermissionsAsync(
        int id, UpdatePermissionsDto dto, string updatedBy, CancellationToken ct = default)
    {
        var role = await _uow.Roles.GetWithPermissionsAsync(id, ct)
            ?? throw new AppException($"Role {id} not found.");

        var newPerms = dto.Permissions.Select(p => new RolePermission
        {
            RoleId    = id,
            ModuleKey = p.ModuleKey,
            CanView   = p.CanView,
            CanAdd    = p.CanAdd,
            CanUpdate = p.CanUpdate,
            CanDelete = p.CanDelete,
            CreatedBy = updatedBy,
        }).ToList();

        await _uow.Roles.ReplacePermissionsAsync(id, newPerms, ct);

        role.UpdatedAt = DateTime.UtcNow;
        role.UpdatedBy = updatedBy;
        await _uow.Roles.UpdateAsync(role, ct);
        await _uow.CommitAsync(ct);

        var updated = await _uow.Roles.GetWithPermissionsAsync(id, ct)!;
        return ToDetailDto(updated!);
    }

    public async Task<IEnumerable<RoleUserDto>> GetAllUsersAsync(CancellationToken ct = default)
    {
        var users = await _uow.Users.GetAllActiveAsync(ct);
        return users.Select(ToUserDto);
    }

    public async Task<IEnumerable<RoleUserDto>> GetUsersForRoleAsync(int roleId, CancellationToken ct = default)
    {
        var users = await _uow.Users.GetByRoleIdAsync(roleId, ct);
        return users.Select(ToUserDto);
    }

    public async Task AssignUserAsync(int roleId, int userId, string assignedBy, CancellationToken ct = default)
    {
        var role = await _uow.Roles.GetByIdAsync(roleId, ct)
            ?? throw new AppException($"Role {roleId} not found.");

        var users = await _uow.Users.FindAsync(u => u.Id == userId, ct);
        var user  = users.FirstOrDefault()
            ?? throw new AppException($"User {userId} not found.");

        user.RoleId    = roleId;
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = assignedBy;
        await _uow.Users.UpdateAsync(user, ct);
        await _uow.CommitAsync(ct);
    }

    public async Task RemoveUserAsync(int roleId, int userId, string removedBy, CancellationToken ct = default)
    {
        var users = await _uow.Users.FindAsync(u => u.Id == userId, ct);
        var user  = users.FirstOrDefault()
            ?? throw new AppException($"User {userId} not found.");

        if (user.RoleId != roleId)
            throw new AppException($"User {userId} is not assigned to role {roleId}.");

        user.RoleId    = null;
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = removedBy;
        await _uow.Users.UpdateAsync(user, ct);
        await _uow.CommitAsync(ct);
    }

    public async Task<Dictionary<string, ModulePermissionDto>> GetUserPermissionsAsync(
        int userId, string userRole, CancellationToken ct = default)
    {
        // Admins get full access to everything
        if (AdminRoles.Contains(userRole))
        {
            return AllModuleKeys.ToDictionary(
                k => k,
                k => new ModulePermissionDto
                {
                    ModuleKey = k,
                    CanView   = true,
                    CanAdd    = true,
                    CanUpdate = true,
                    CanDelete = true,
                });
        }

        // Look up the user's assigned permission role
        var users = await _uow.Users.FindAsync(u => u.Id == userId, ct);
        var user  = users.FirstOrDefault();
        if (user?.RoleId is null)
            return new Dictionary<string, ModulePermissionDto>();

        var perms = await _uow.Roles.GetPermissionsForRoleAsync(user.RoleId.Value, ct);
        return perms.ToDictionary(
            p => p.ModuleKey,
            p => new ModulePermissionDto
            {
                ModuleKey = p.ModuleKey,
                CanView   = p.CanView,
                CanAdd    = p.CanAdd,
                CanUpdate = p.CanUpdate,
                CanDelete = p.CanDelete,
            });
    }

    // ── Mappers ──────────────────────────────────────────────────────────────

    private static RoleDto ToDto(Role r) => new()
    {
        Id          = r.Id,
        Name        = r.Name,
        Description = r.Description,
        UserCount   = r.Users.Count,
    };

    private static RoleUserDto ToUserDto(Domain.Entities.User u) => new()
    {
        Id         = u.Id,
        Username   = u.Username,
        Email      = u.Email,
        Name       = u.Username,
        SystemRole = u.Role,
    };

    private static RoleWithPermissionsDto ToDetailDto(Role r) => new()
    {
        Id          = r.Id,
        Name        = r.Name,
        Description = r.Description,
        Permissions = r.Permissions.Select(p => new ModulePermissionDto
        {
            ModuleKey = p.ModuleKey,
            CanView   = p.CanView,
            CanAdd    = p.CanAdd,
            CanUpdate = p.CanUpdate,
            CanDelete = p.CanDelete,
        }),
    };
}
