using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApi.Application.Common.Models;
using PayrollApi.Application.DTOs.Role;
using PayrollApi.Application.Services;
using PayrollApi.Application.Services.Interfaces;

namespace PayrollApi.API.Controllers.v1;

[ApiVersion("1.0")]
[Authorize(Policy = "PayrollAdmin")]
public class RolesController : BaseController
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService) => _roleService = roleService;

    /// <summary>Get all roles (with user counts).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<RoleDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var roles = await _roleService.GetAllAsync(ct);
        return Ok(ApiResponse<IEnumerable<RoleDto>>.Ok(roles));
    }

    /// <summary>Get a single role with its full permission matrix.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<RoleWithPermissionsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var role = await _roleService.GetWithPermissionsAsync(id, ct);
        return Ok(ApiResponse<RoleWithPermissionsDto>.Ok(role));
    }

    /// <summary>Returns the list of all known module keys the system supports.</summary>
    [HttpGet("modules")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<string>>), StatusCodes.Status200OK)]
    public IActionResult GetModules() =>
        Ok(ApiResponse<IReadOnlyList<string>>.Ok(RoleService.AllModuleKeys));

    /// <summary>Create a new role.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateRoleDto dto, CancellationToken ct)
    {
        var role = await _roleService.CreateAsync(dto, CurrentUser, ct);
        return CreatedAtAction(nameof(GetById), new { id = role.Id }, ApiResponse<RoleDto>.Ok(role, "Role created."));
    }

    /// <summary>Update a role's name and description.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRoleDto dto, CancellationToken ct)
    {
        var role = await _roleService.UpdateAsync(id, dto, CurrentUser, ct);
        return Ok(ApiResponse<RoleDto>.Ok(role, "Role updated."));
    }

    /// <summary>Soft-delete a role. Affected users lose their permission role assignment.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _roleService.DeleteAsync(id, CurrentUser, ct);
        return NoContent();
    }

    /// <summary>Replace the full permission matrix for a role.</summary>
    [HttpPut("{id:int}/permissions")]
    [ProducesResponseType(typeof(ApiResponse<RoleWithPermissionsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePermissions(int id, [FromBody] UpdatePermissionsDto dto, CancellationToken ct)
    {
        var role = await _roleService.UpdatePermissionsAsync(id, dto, CurrentUser, ct);
        return Ok(ApiResponse<RoleWithPermissionsDto>.Ok(role, "Permissions updated."));
    }

    // ── User assignment ──────────────────────────────────────────────────────

    /// <summary>All active users — for the "Add user to role" picker.</summary>
    [HttpGet("users")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<RoleUserDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUsers(CancellationToken ct)
    {
        var users = await _roleService.GetAllUsersAsync(ct);
        return Ok(ApiResponse<IEnumerable<RoleUserDto>>.Ok(users));
    }

    /// <summary>Users currently assigned to a specific role.</summary>
    [HttpGet("{id:int}/users")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<RoleUserDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsersForRole(int id, CancellationToken ct)
    {
        var users = await _roleService.GetUsersForRoleAsync(id, ct);
        return Ok(ApiResponse<IEnumerable<RoleUserDto>>.Ok(users));
    }

    /// <summary>Assign a user to a role.</summary>
    [HttpPost("{id:int}/users")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignUser(int id, [FromBody] AssignUserDto dto, CancellationToken ct)
    {
        await _roleService.AssignUserAsync(id, dto.UserId, CurrentUser, ct);
        return NoContent();
    }

    /// <summary>Remove a user's assignment from a role.</summary>
    [HttpDelete("{id:int}/users/{userId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveUser(int id, int userId, CancellationToken ct)
    {
        await _roleService.RemoveUserAsync(id, userId, CurrentUser, ct);
        return NoContent();
    }
}
