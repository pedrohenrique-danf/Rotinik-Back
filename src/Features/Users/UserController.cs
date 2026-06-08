using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Rotinik.Features.Users.DTOs;
using Rotinik.Core.Extensions;
using Microsoft.EntityFrameworkCore;

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
    public async Task<IActionResult> RefreshToken(TokenDto dto)
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

        var response = await _userService.GetCurrentUserAsync(currentUserId);
        return Ok(response);
    }

    [HttpGet("profile/{username}")]
    [Tags(ProfileTag)]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPublicProfile(string username)
    {
        var profile = await _userService.GetPublicProfileAsync(username);
        return Ok(profile);
    }

    [HttpGet("rank")]
    [Tags(ProfileTag)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRank([FromQuery] int limit = 100)
    {
        if (limit > 500) limit = 500; 

        var rank = await _userService.GetTopRankedUsersAsync(limit);
        return Ok(rank);
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

    [Authorize]
    [HttpPost("upgrade-premium")]
    [Tags(PremiumTag)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpgradePremium()
    {
        var currentUserId = User.GetCurrentUserId();
        await _userService.UpgradeToPremiumAsync(currentUserId);
        return Ok(new { message = "Seja bem-vindo ao Premium!" });
    }

    [HttpGet("debug-list")]
    [AllowAnonymous]
    public async Task<IActionResult> DebugList([FromServices] Rotinik.Core.Data.AppDbContext context)
    {
        var users = await context.Users.AsNoTracking().ToListAsync();
        return Ok(users);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet("admin/users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ListUsersAdmin([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var result = await _userService.ListUsersAdminAsync(search, page, pageSize);
        return Ok(result);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("admin/users/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUserAdmin(int id, AdminUserUpdateDto dto)
    {
        await _userService.UpdateUserAdminAsync(id, dto);
        return NoContent();
    }
}