#nullable enable
using RotinikApi.DTOs.Requests;
using RotinikApi.DTOs.Requests.Auth;
using RotinikApi.DTOs.Responses;
using RotinikApi.DTOs.Responses.Auth;

namespace RotinikApi.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponse>> GetAllAsync();
        Task<UserResponse> CreateAsync(UserCreateRequest dto);
        Task DeleteAsync(int id);
        Task<UserResponse> LoginAsync(AuthRequest dto);
        Task<AuthResponse?> AuthenticateAsync(AuthRequest dto);
        Task<UserResponse?> GetMeAsync(int userId);
    }
}