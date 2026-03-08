namespace PayrollApi.Application.DTOs.Auth;

/// <summary>Request DTO for creating a new application user.</summary>
public sealed class CreateUserDto
{
    public string Username { get; init; } = string.Empty;
    public string Email    { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    /// <summary>System role: Admin, PayrollAdmin, HrStaff, Manager.</summary>
    public string Role { get; init; } = "HrStaff";
    /// <summary>Optional permission role ID for granular module access.</summary>
    public int? RoleId { get; init; }
}
