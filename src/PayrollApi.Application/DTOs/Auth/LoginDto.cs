namespace PayrollApi.Application.DTOs.Auth;

public sealed class LoginDto
{
    public string UsernameOrEmail { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
