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
    private readonly IUserCommandService _commandService;
    private readonly IUserQueryService _queryService;

    public UserController(IUserCommandService commandService, IUserQueryService queryService)
    {
        _commandService = commandService;
        _queryService = queryService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateUser(UserRegistrationDto dto)
    {
        await _commandService.CreateUserAsync(dto);
        return StatusCode(201, new { message = "User created" });
    }

    [HttpGet("profile/{username}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPublicProfile(string username)
    {
        var profile = await _queryService.GetPublicProfileAsync(username);
        return Ok(profile);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(int id, UserUpdateDto dto)
    {
        var currentUserId = GetCurrentUserId();
        await _commandService.UpdateUserAsync(id, currentUserId, dto);
        return NoContent();  
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var currentUserId = GetCurrentUserId();
        await _commandService.DeleteUserAsync(id, currentUserId);
        return NoContent();
    }

    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == 0)
            return Unauthorized(new { message = "Invalid token payload." });

        var response = await _queryService.GetCurrentUserAsync(currentUserId);
        return Ok(response);
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int.TryParse(userIdClaim, out int userId);
        return userId;
    }

    [Authorize(Policy = "PremiumOnly")]
    [HttpGet("conteudo-vip")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetPremiumContent()
    {
        return Ok(new 
        { 
            message = "Bem-vindo à área VIP! Este conteúdo é exclusivo para assinantes Premium." 
        });
    }

    [Authorize]
    [HttpPost("premium")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpgradeToPremium()
    {
        var currentUserId = GetCurrentUserId();
        
        await _commandService.ActivatePremiumAsync(currentUserId);
        
        return Ok(new { message = "Parabéns! Sua conta agora é Premium." });
    }
}