using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Rotinik.Features.Users.DTOs;
using Rotinik.Core.Extensions;

namespace Rotinik.Features.Users;

[Route("api/user")]  
[ApiController]
public class UserController : ControllerBase
{
    private const string AuthTag = "User (Autenticação)";
    private const string AccountTag = "User (Gerenciamento)";
    private const string ProfileTag = "User (Exibição de Perfil)";
    private const string PremiumTag = "User (Acesso Premium)";

    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    [Tags(AuthTag)]
    [EnableRateLimiting("LoginPolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Login(UserLoginDto dto)
    {
        var response = await _userService.LoginAsync(dto);
        return Ok(new { data = response, message = "Login successful!" });
    }

    [HttpPost("refresh-token")]
    [Tags(AuthTag)]
    [EnableRateLimiting("LoginPolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto dto)
    {
        var response = await _userService.RefreshTokenAsync(dto);
        return Ok(new { data = response, message = "Token refreshed successfully!" });
    }

    [HttpPost]
    [Tags(AccountTag)]
    [EnableRateLimiting("LoginPolicy")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> CreateUser(UserRegistrationDto dto)
    {
        await _userService.CreateUserAsync(dto);
        return StatusCode(201, new { message = "User created" });
    }

    [Authorize]
    [HttpPut("{id}")]
    [Tags(AccountTag)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(int id, UserUpdateDto dto)
    {
        var currentUserId = User.GetCurrentUserId();
        await _userService.UpdateUserAsync(id, currentUserId, dto);
        return NoContent();  
    }

    [Authorize]
    [HttpDelete("{id}")]
    [Tags(AccountTag)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var currentUserId = User.GetCurrentUserId();
        await _userService.DeleteUserAsync(id, currentUserId);
        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    [Tags(AccountTag)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var currentUserId = User.GetCurrentUserId();
        if (currentUserId == 0)
            return Unauthorized(new { message = "Invalid token payload." });

        var response = await _userService.GetCurrentUserAsync(currentUserId);
        return Ok(response);
    }

    [HttpGet("profile/{username}")]
    [Tags(ProfileTag)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPublicProfile(string username)
    {
        var profile = await _userService.GetPublicProfileAsync(username);
        return Ok(profile);
    }

    [Authorize(Policy = "PremiumOnly")]
    [HttpGet("conteudo-vip")]
    [Tags(PremiumTag)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetPremiumContent()
    {
        return Ok(new 
        { 
            message = "Premium Content Accessed." 
        });
    }
}