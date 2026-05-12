#nullable enable
using RotinikApi.DTOs.Requests;
using RotinikApi.DTOs.Requests.Auth;
using RotinikApi.DTOs.Responses;
using RotinikApi.DTOs.Responses.Auth;

namespace RotinikApi.Services
{
    public interface IUserService
    {
        System.Threading.Tasks.Task<IEnumerable<UserResponse>> GetAllAsync();
        System.Threading.Tasks.Task<UserResponse> CreateAsync(UserCreateRequest dto);
        System.Threading.Tasks.Task DeleteAsync(int id);
        System.Threading.Tasks.Task<UserResponse> LoginAsync(AuthRequest dto);
        System.Threading.Tasks.Task<AuthResponse?> AuthenticateAsync(AuthRequest dto);
        System.Threading.Tasks.Task<UserResponse?> GetMeAsync(int userId);
    }
}