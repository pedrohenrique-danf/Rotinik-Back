using Microsoft.AspNetCore.Mvc;
using RotinikApi.DTOs.Requests;
using RotinikApi.Services;

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
    }
}