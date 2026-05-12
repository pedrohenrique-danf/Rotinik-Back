using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RotinikApi.DTOs.Requests;
using RotinikApi.DTOs.Requests.Auth;
using RotinikApi.Services;
using RotinikApi.DTOs.Responses.Auth;

namespace RotinikApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _service;

        public UsuarioController(IUsuarioService service)
        {
            _service = service;
        }

        /// <summary>Lista todos os usuários. Requer autenticação.</summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Listar()
        {
            var usuarios = await _service.ListarAsync();
            return Ok(usuarios);
        }

        /// <summary>Retorna os dados do usuário autenticado pelo token JWT.</summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            // Extrai o ID do usuário do Claim "sub" do token JWT
            var subClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                        ?? User.FindFirst("sub");

            if (subClaim == null || !int.TryParse(subClaim.Value, out var usuarioId))
                return Unauthorized(new { Mensagem = "Token inválido ou expirado." });

            var usuario = await _service.MeAsync(usuarioId);

            if (usuario == null)
                return NotFound(new { Mensagem = "Usuário não encontrado." });

            return Ok(usuario);
        }

        /// <summary>Cria um novo usuário. Público (sem autenticação).</summary>
        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] UsuarioCriarRequest dto)
        {
            var usuario = await _service.CriarAsync(dto);
            return CreatedAtAction(nameof(Listar), new { id = usuario.Id }, usuario);
        }

        /// <summary>Remove um usuário pelo ID. Requer autenticação.</summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Remover(int id)
        {
            await _service.RemoverAsync(id);
            return NoContent();
        }

        /// <summary>Autentica o usuário e retorna um token JWT. Público.</summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequest dto)
        {
            var response = await _service.AutenticarAsync(dto);

            if (response == null)
                return Unauthorized(new { Mensagem = "E-mail ou senha inválidos." });

            return Ok(response);
        }
    }
}