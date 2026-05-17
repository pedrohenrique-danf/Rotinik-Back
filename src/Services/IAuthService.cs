using Rotinik.DTOs.User;

namespace Rotinik.Services;

public interface IAuthService
{
    Task<string?> LoginAsync(UserLoginDto dto);
}