using PayrollApi.Application.DTOs.Role;

namespace PayrollApi.Application.DTOs.Auth;

public sealed class AuthResponseDto
{
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public AuthUserDto User { get; init; } = new();
}

public sealed class AuthUserDto
{
    public int Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;

    /// <summary>
    /// Per-module permissions keyed by module id (e.g. "attendance").
    /// Admins receive all-true entries for every module.
    /// </summary>
    public Dictionary<string, ModulePermissionDto> Permissions { get; init; } = new();
}
