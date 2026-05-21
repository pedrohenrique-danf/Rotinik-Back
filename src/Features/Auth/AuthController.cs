using Microsoft.AspNetCore.Mvc;
using Rotinik.Features.Users.DTOs;

namespace Rotinik.Features.Auth;

[Route("api/auth")]  
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(UserLoginDto dto)
    {
        var response = await _authService.LoginAsync(dto);
        return Ok(new { data = response, message = "Login successful!" });
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto dto)
    {
        var response = await _authService.RefreshTokenAsync(dto);
        return Ok(new { data = response, message = "Token refreshed successfully!" });
    }
}