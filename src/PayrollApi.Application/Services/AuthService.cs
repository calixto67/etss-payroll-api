using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PayrollApi.Application.Common.Exceptions;
using PayrollApi.Application.DTOs.Auth;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class AuthService : IAuthService
{
    private const string SP = "sp_User";

    private readonly ISqlExecutor _sql;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;
    private readonly IRoleService _roleService;

    public AuthService(
        ISqlExecutor sql,
        IConfiguration configuration,
        ILogger<AuthService> logger,
        IRoleService roleService)
    {
        _sql           = sql;
        _configuration = configuration;
        _logger        = logger;
        _roleService   = roleService;
    }

    // ── Internal row types for Dapper mapping ────────────────────────────

    private sealed class LoginRow
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public bool IsActive { get; set; }
        public string Role { get; set; } = "";
        public int? RoleId { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }

    // ── Public methods ───────────────────────────────────────────────────

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _sql.QueryFirstOrDefaultAsync<LoginRow>(
                SP, new { ActionType = "GET_FOR_LOGIN", UsernameOrEmail = dto.UsernameOrEmail }, cancellationToken);

            if (user is null || !user.IsActive)
                throw new AppException("Invalid username/email or password.");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new AppException("Invalid username/email or password.");

            var (token, expiresAt) = GenerateJwtToken(user);

            // Load granular module permissions for this user
            var permissions = await _roleService.GetUserPermissionsAsync(user.Id, user.Role, cancellationToken);

            _logger.LogInformation("User {Email} logged in successfully", user.Email);

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                User = new AuthUserDto
                {
                    Id          = user.Id,
                    Username    = user.Username,
                    Email       = user.Email,
                    Name        = user.Username,
                    Role        = user.Role,
                    Permissions = permissions,
                }
            };
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    private (string token, DateTime expiresAt) GenerateJwtToken(LoginRow user)
    {
        var secretKey = _configuration["Jwt:SecretKey"]
            ?? throw new InvalidOperationException("JWT SecretKey is not configured.");

        var key     = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds   = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry  = int.TryParse(_configuration["Jwt:ExpiryMinutes"], out var mins) ? mins : 60;
        var expires = DateTime.Now.AddMinutes(expiry);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(ClaimTypes.Name,               user.Username),
            new Claim(ClaimTypes.Role,               user.Role),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),
        };

        var token = new JwtSecurityToken(
            issuer:   _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims:   claims,
            expires:  expires,
            signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
}
