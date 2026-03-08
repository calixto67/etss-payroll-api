using Microsoft.Data.SqlClient;
using PayrollApi.Application.Common.Exceptions;
using PayrollApi.Application.DTOs.Auth;
using PayrollApi.Application.DTOs.Role;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class UserService : IUserService
{
    private const string SP = "sp_User";

    private readonly ISqlExecutor _sql;

    public UserService(ISqlExecutor sql) => _sql = sql;

    // ── Internal row types for Dapper mapping ────────────────────────────

    private sealed class UserRow
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
    }

    // ── Public methods ───────────────────────────────────────────────────

    public async Task<RoleUserDto> CreateAsync(CreateUserDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        try
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var row = await _sql.QueryFirstOrDefaultAsync<UserRow>(SP, new
            {
                ActionType   = "CREATE",
                Username     = dto.Username.Trim(),
                Email        = dto.Email.Trim(),
                PasswordHash = passwordHash,
                Role         = dto.Role.Trim(),
                RoleId       = dto.RoleId,
                CreatedBy    = createdBy,
            }, cancellationToken) ?? throw new AppException("Failed to create user.");

            return new RoleUserDto
            {
                Id         = row.Id,
                Username   = row.Username,
                Email      = row.Email,
                Name       = row.Username,
                SystemRole = row.Role,
            };
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task ResetPasswordAsync(int userId, string newPassword, string updatedBy, CancellationToken cancellationToken = default)
    {
        try
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            await _sql.ExecuteAsync(SP, new
            {
                ActionType   = "RESET_PASSWORD",
                Id           = userId,
                PasswordHash = passwordHash,
                UpdatedBy    = updatedBy,
            }, cancellationToken);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }
}
