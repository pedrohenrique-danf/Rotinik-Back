#nullable enable
using RotinikApi.DTOs.Requests;
using RotinikApi.DTOs.Requests.Auth;
using RotinikApi.DTOs.Responses;
using RotinikApi.DTOs.Responses.Auth;

namespace RotinikApi.Services
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioResponse>> ListarAsync();
        Task<UsuarioResponse> CriarAsync(UsuarioCriarRequest dto);
        Task RemoverAsync(int id);
        Task<UsuarioResponse> LoginAsync(AuthRequest dto);
        Task<AuthResponse?> AutenticarAsync(AuthRequest dto);
    }
}