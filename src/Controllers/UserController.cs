using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotinik.DTOs.User;
using Rotinik.Services;
using System.Security.Claims;

namespace Rotinik.Controllers;

[Route("api/user")]  
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateUser(UserRegistrationDto dto)
    {
        await _userService.CreateUserAsync(dto);
        return StatusCode(201, new { message = "User created" });
    }

    [HttpGet("profile/{username}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPublicProfile(string username)
    {
        var profile = await _userService.GetPublicProfileAsync(username);
        return Ok(profile);
    }

    [Authorize]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateUser(int id, UserUpdateDto dto)
    {
        await _userService.UpdateUserAsync(id, GetCurrentUserId(), dto);
        return NoContent();  
    }

    [Authorize]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await _userService.DeleteUserAsync(id, GetCurrentUserId());
        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == 0) return Unauthorized(new { message = "Invalid token payload." });

        var response = await _userService.GetCurrentUserAsync(currentUserId);
        return Ok(response);
    }

    [Authorize(Policy = "PremiumOnly")]
    [HttpGet("conteudo-vip")]
    public IActionResult GetPremiumContent()
    {
        return Ok(new { message = "Bem-vindo à área VIP!" });
    }

    [Authorize]
    [HttpPost("premium")]
    public async Task<IActionResult> UpgradeToPremium()
    {
        await _userService.ActivatePremiumAsync(GetCurrentUserId());
        return Ok(new { message = "Parabéns! Sua conta agora é Premium." });
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int.TryParse(userIdClaim, out int userId);
        return userId;
    }
}