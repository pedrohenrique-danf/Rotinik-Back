using Rotinik.DTOs.User;

namespace Rotinik.Services;

public interface IUserQueryService
{
    Task<UserProfileDto?> GetPublicProfileAsync(string username);
    Task<UserResponseDto?> GetCurrentUserAsync(int userId);
}