using PayrollApi.Application.DTOs.Auth;
using PayrollApi.Application.DTOs.Role;

namespace PayrollApi.Application.Services.Interfaces;

public interface IUserService
{
    /// <summary>Create a new application user.</summary>
    Task<RoleUserDto> CreateAsync(CreateUserDto dto, string createdBy, CancellationToken cancellationToken = default);

    /// <summary>Reset a user's password.</summary>
    Task ResetPasswordAsync(int userId, string newPassword, string updatedBy, CancellationToken cancellationToken = default);
}
