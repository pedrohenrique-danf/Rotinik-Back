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

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var usuarios = await _service.ListarAsync();
            return Ok(usuarios);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] UsuarioCriarRequest dto)
        {
            var usuario = await _service.CriarAsync(dto);
            return CreatedAtAction(nameof(Listar), new { id = usuario.Id }, usuario);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remover(int id)
        {
            await _service.RemoverAsync(id);
            return NoContent();
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequest dto)
        {
            // 1. O Controller delega a validação para o Service
            var response = await _service.AutenticarAsync(dto);

            if (response == null)
            {
                return Unauthorized(new { Mensagem = "E-mail ou senha inválidos." });
            }

            return Ok(response);
        }
    }
}