#nullable enable
using RotinikApi.DTOs.Responses;

namespace RotinikApi.DTOs.Responses.Auth
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UsuarioResponse Usuario { get; set; } = null!;
    }
}
