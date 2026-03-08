using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApi.Application.Common.Models;
using PayrollApi.Application.DTOs.Auth;
using PayrollApi.Application.DTOs.Role;
using PayrollApi.Application.Services.Interfaces;

namespace PayrollApi.API.Controllers.v1;

[ApiVersion("1.0")]
[Authorize(Policy = "PayrollAdmin")]
public class UsersController : BaseController
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;

    public UsersController(IUserService userService, IRoleService roleService)
    {
        _userService   = userService;
        _roleService   = roleService;
    }

    /// <summary>Get all active users.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<RoleUserDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var users = await _roleService.GetAllUsersAsync(ct);
        return Ok(ApiResponse<IEnumerable<RoleUserDto>>.Ok(users));
    }

    /// <summary>Reset a user's password. Admin only.</summary>
    [HttpPut("{id}/reset-password")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetPasswordDto dto, CancellationToken ct)
    {
        await _userService.ResetPasswordAsync(id, dto.NewPassword, CurrentUser, ct);
        return Ok(ApiResponse<object>.Ok(null!, "Password reset successfully."));
    }

    /// <summary>Create a new application user.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<RoleUserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto, CancellationToken ct)
    {
        var user = await _userService.CreateAsync(dto, CurrentUser, ct);
        return CreatedAtAction(nameof(GetAll), ApiResponse<RoleUserDto>.Ok(user, "User created."));
    }
}
