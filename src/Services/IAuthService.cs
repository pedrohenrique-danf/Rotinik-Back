using Rotinik.DTOs.User;

namespace Rotinik.Services;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(UserLoginDto dto);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto);
}