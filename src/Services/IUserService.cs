using Rotinik.DTOs.User;

namespace Rotinik.Services;

public interface IUserService
{
    Task CreateUserAsync(UserRegistrationDto dto);
    Task<UserProfileDto?> GetPublicProfileAsync(string username);
    Task UpdateUserAsync(int id, int currentUserId, UserUpdateDto dto);
    Task DeleteUserAsync(int id, int currentUserId);
    Task<string?> LoginAsync(UserLoginDto dto);
    Task<UserResponseDto?> GetCurrentUserAsync(int userId);
}