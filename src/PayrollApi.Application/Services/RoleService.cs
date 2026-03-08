using System.Text.Json;
using Microsoft.Data.SqlClient;
using PayrollApi.Application.Common.Exceptions;
using PayrollApi.Application.DTOs.Role;
using PayrollApi.Application.Services.Interfaces;
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
        "government-reports", "reports", "setup", "user-roles", "users",
        "work-schedules"
    };

    private static readonly HashSet<string> AdminRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "Admin"
    };

    private const string SP = "sp_Role";

    private readonly ISqlExecutor _sql;

    public RoleService(ISqlExecutor sql) => _sql = sql;

    // ── Internal row types for Dapper mapping ────────────────────────────

    private sealed class RoleRow
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public int UserCount { get; set; }
    }

    private sealed class RolePermissionRow
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public string ModuleKey { get; set; } = "";
        public bool CanView { get; set; }
        public bool CanAdd { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }

    private sealed class ModulePermissionRow
    {
        public string ModuleKey { get; set; } = "";
        public bool CanView { get; set; }
        public bool CanAdd { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }

    private sealed class RoleUserRow
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Name { get; set; } = "";
        public string SystemRole { get; set; } = "";
    }

    // ── Public methods ───────────────────────────────────────────────────

    public async Task<IEnumerable<RoleDto>> GetAllAsync(CancellationToken ct = default)
    {
        try
        {
            var rows = await _sql.QueryAsync<RoleRow>(SP, new { ActionType = "GET_ALL" }, ct);
            return rows.Select(r => new RoleDto
            {
                Id          = r.Id,
                Name        = r.Name,
                Description = r.Description,
                UserCount   = r.UserCount,
            });
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<RoleWithPermissionsDto> GetWithPermissionsAsync(int id, CancellationToken ct = default)
    {
        try
        {
            // GET_WITH_PERMISSIONS returns a flat join of role + permissions (one row per permission)
            var rows = await _sql.QueryAsync<RolePermissionRow>(
                SP, new { ActionType = "GET_WITH_PERMISSIONS", Id = id }, ct);

            var list = rows.ToList();
            if (list.Count == 0)
                throw new AppException($"Role {id} not found.");

            var first = list[0];
            return new RoleWithPermissionsDto
            {
                Id          = first.Id,
                Name        = first.Name,
                Description = first.Description,
                Permissions = list
                    .Where(r => !string.IsNullOrEmpty(r.ModuleKey))
                    .Select(r => new ModulePermissionDto
                    {
                        ModuleKey = r.ModuleKey,
                        CanView   = r.CanView,
                        CanAdd    = r.CanAdd,
                        CanUpdate = r.CanUpdate,
                        CanDelete = r.CanDelete,
                    }),
            };
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<RoleDto> CreateAsync(CreateRoleDto dto, string createdBy, CancellationToken ct = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<RoleRow>(SP, new
            {
                ActionType  = "CREATE",
                Name        = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                CreatedBy   = createdBy,
            }, ct) ?? throw new AppException("Failed to create role.");

            return new RoleDto
            {
                Id          = row.Id,
                Name        = row.Name,
                Description = row.Description,
                UserCount   = row.UserCount,
            };
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<RoleDto> UpdateAsync(int id, UpdateRoleDto dto, string updatedBy, CancellationToken ct = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<RoleRow>(SP, new
            {
                ActionType  = "UPDATE",
                Id          = id,
                Name        = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                UpdatedBy   = updatedBy,
            }, ct) ?? throw new AppException($"Role {id} not found.");

            return new RoleDto
            {
                Id          = row.Id,
                Name        = row.Name,
                Description = row.Description,
                UserCount   = row.UserCount,
            };
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task DeleteAsync(int id, string deletedBy, CancellationToken ct = default)
    {
        try
        {
            await _sql.ExecuteAsync(SP, new
            {
                ActionType = "DELETE",
                Id         = id,
                DeletedBy  = deletedBy,
            }, ct);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<RoleWithPermissionsDto> UpdatePermissionsAsync(
        int id, UpdatePermissionsDto dto, string updatedBy, CancellationToken ct = default)
    {
        try
        {
            var permissionsJson = JsonSerializer.Serialize(dto.Permissions);

            await _sql.ExecuteAsync(SP, new
            {
                ActionType      = "UPDATE_PERMISSIONS",
                Id              = id,
                PermissionsJson = permissionsJson,
                UpdatedBy       = updatedBy,
            }, ct);

            // Return the updated role with permissions
            return await GetWithPermissionsAsync(id, ct);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<IEnumerable<RoleUserDto>> GetAllUsersAsync(CancellationToken ct = default)
    {
        try
        {
            var rows = await _sql.QueryAsync<RoleUserRow>(SP, new { ActionType = "GET_ALL_USERS" }, ct);
            return rows.Select(r => new RoleUserDto
            {
                Id         = r.Id,
                Username   = r.Username,
                Email      = r.Email,
                Name       = r.Name,
                SystemRole = r.SystemRole,
            });
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<IEnumerable<RoleUserDto>> GetUsersForRoleAsync(int roleId, CancellationToken ct = default)
    {
        try
        {
            var rows = await _sql.QueryAsync<RoleUserRow>(
                SP, new { ActionType = "GET_USERS_FOR_ROLE", Id = roleId }, ct);
            return rows.Select(r => new RoleUserDto
            {
                Id         = r.Id,
                Username   = r.Username,
                Email      = r.Email,
                Name       = r.Name,
                SystemRole = r.SystemRole,
            });
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task AssignUserAsync(int roleId, int userId, string assignedBy, CancellationToken ct = default)
    {
        try
        {
            await _sql.ExecuteAsync(SP, new
            {
                ActionType = "ASSIGN_USER",
                Id         = roleId,
                UserId     = userId,
                AssignedBy = assignedBy,
            }, ct);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task RemoveUserAsync(int roleId, int userId, string removedBy, CancellationToken ct = default)
    {
        try
        {
            await _sql.ExecuteAsync(SP, new
            {
                ActionType = "REMOVE_USER",
                Id         = roleId,
                UserId     = userId,
                RemovedBy  = removedBy,
            }, ct);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
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

        try
        {
            var rows = await _sql.QueryAsync<ModulePermissionRow>(
                SP, new { ActionType = "GET_USER_PERMISSIONS", UserId = userId, UserRole = userRole }, ct);

            return rows.ToDictionary(
                r => r.ModuleKey,
                r => new ModulePermissionDto
                {
                    ModuleKey = r.ModuleKey,
                    CanView   = r.CanView,
                    CanAdd    = r.CanAdd,
                    CanUpdate = r.CanUpdate,
                    CanDelete = r.CanDelete,
                });
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }
}
