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
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        /// <summary>Lists all users. Requires authentication.</summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var users = await _service.GetAllAsync();
            return Ok(users);
        }

        /// <summary>Returns the authenticated user data via JWT token.</summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            // Extracts user ID from JWT "sub" claim
            var subClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                        ?? User.FindFirst("sub");

            if (subClaim == null || !int.TryParse(subClaim.Value, out var userId))
                return Unauthorized(new { Message = "Invalid or expired token." });

            var user = await _service.GetMeAsync(userId);

            if (user == null)
                return NotFound(new { Message = "User not found." });

            return Ok(user);
        }

        /// <summary>Creates a new user. Public (no authentication).</summary>
        [HttpPost("register")]
        public async Task<IActionResult> Create([FromBody] UserCreateRequest dto)
        {
            var user = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetAll), new { id = user.Id }, user);
        }

        /// <summary>Deletes a user by ID. Requires authentication.</summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>Authenticates the user and returns a JWT token. Public.</summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequest dto)
        {
            var response = await _service.AuthenticateAsync(dto);

            if (response == null)
                return Unauthorized(new { Message = "Invalid email or password." });

            return Ok(response);
        }
    }
}