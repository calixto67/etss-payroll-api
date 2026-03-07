using PayrollApi.Application.DTOs.Auth;

namespace PayrollApi.Application.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
}
