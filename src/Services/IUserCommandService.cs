using Rotinik.DTOs.User;

namespace Rotinik.Services;

public interface IUserCommandService
{
    Task CreateUserAsync(UserRegistrationDto dto);
    Task UpdateUserAsync(int id, int currentUserId, UserUpdateDto dto);
    Task DeleteUserAsync(int id, int currentUserId);
}