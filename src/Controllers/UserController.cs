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
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateUser(UserRegistrationDto dto)
    {
        await _userService.CreateUserAsync(dto);
        return StatusCode(201, new { message = "User created" });
    }

    [HttpGet("profile/{username}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPublicProfile(string username)
    {
        var profile = await _userService.GetPublicProfileAsync(username);
        return Ok(profile);
    }

    [Authorize]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(int id, UserUpdateDto dto)
    {
        var currentUserId = GetCurrentUserId();
        await _userService.UpdateUserAsync(id, currentUserId, dto);
        return NoContent();  
    }

    [Authorize]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var currentUserId = GetCurrentUserId();
        await _userService.DeleteUserAsync(id, currentUserId);
        return NoContent();
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(UserLoginDto dto)
    {
        var token = await _userService.LoginAsync(dto);
        return Ok(new { token = token, message = "Login successful!" });
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == 0)
            return Unauthorized(new { message = "Invalid token payload." });

        var response = await _userService.GetCurrentUserAsync(currentUserId);
        return Ok(response);
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int.TryParse(userIdClaim, out int userId);
        return userId;
    }
}