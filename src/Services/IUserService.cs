using Rotinik.DTOs.User;

namespace Rotinik.Services;

public interface IUserService
{
    void CreateUser(UserRegistrationDto dto);
    UserProfileDto? GetPublicProfile(string username);
    void UpdateUser(int id, int currentUserId, UserUpdateDto dto);
    void DeleteUser(int id, int currentUserId);
    string? Login(UserLoginDto dto);
    UserResponseDto? GetCurrentUser(int userId);
}