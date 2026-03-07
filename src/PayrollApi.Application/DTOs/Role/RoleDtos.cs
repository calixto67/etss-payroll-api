namespace PayrollApi.Application.DTOs.Role;

// ── Read DTOs ────────────────────────────────────────────────────────────────

public sealed class RoleDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int UserCount { get; init; }
}

public sealed class ModulePermissionDto
{
    public string ModuleKey { get; init; } = string.Empty;
    public bool CanView   { get; init; }
    public bool CanAdd    { get; init; }
    public bool CanUpdate { get; init; }
    public bool CanDelete { get; init; }
}

public sealed class RoleWithPermissionsDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public IEnumerable<ModulePermissionDto> Permissions { get; init; } = Enumerable.Empty<ModulePermissionDto>();
}

public sealed class RoleUserDto
{
    public int Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    /// <summary>The legacy string role (Admin, HrStaff, etc.).</summary>
    public string SystemRole { get; init; } = string.Empty;
}

// ── Write DTOs ───────────────────────────────────────────────────────────────

public sealed class AssignUserDto
{
    public int UserId { get; init; }
}

public sealed class CreateRoleDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}

public sealed class UpdateRoleDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}

public sealed class UpdatePermissionDto
{
    public string ModuleKey { get; init; } = string.Empty;
    public bool CanView   { get; init; }
    public bool CanAdd    { get; init; }
    public bool CanUpdate { get; init; }
    public bool CanDelete { get; init; }
}

public sealed class UpdatePermissionsDto
{
    public IEnumerable<UpdatePermissionDto> Permissions { get; init; } = Enumerable.Empty<UpdatePermissionDto>();
}
