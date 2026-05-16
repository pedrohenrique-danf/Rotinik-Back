using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotinik.Data;
using Rotinik.DTOs.User;
using Rotinik.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace Rotinik.Controllers;

[Route("api/user")] 
[ApiController]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public UserController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }


    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult CreateUser(UserRegistrationDto dto)
    {
        if (dto.BirthDate > DateTime.UtcNow)
            return BadRequest(new { message = "Birth date cannot be in the future." });

        if (_context.Users.Any(u => u.UserName == dto.UserName))
            return Conflict(new { message = "UserName in use." });

        if (_context.Users.Any(u => u.Email == dto.Email))
            return Conflict(new { message = "Email in use." });

        var passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$";
        if (!Regex.IsMatch(dto.Password, passwordPattern))
            return BadRequest(new { message = "Password requirements: minimum 8 characters, 1 uppercase, 1 lowercase, 1 number, and 1 symbol." });

        var user = new User
        {
            Name = dto.Name,
            BirthDate = dto.BirthDate.ToUniversalTime(),
            UserName = dto.UserName,
            Email = dto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password) 
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return StatusCode(201, new { message = "User created" });
    }


    [HttpGet("profile/{username}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetPublicProfile(string username)
    {
        var user = _context.Users.SingleOrDefault(u => u.UserName == username);

        if (user == null)
            return NotFound(new { message = "User not found." });

        var profile = new UserProfileDto
        {
            Name = user.Name,
            UserName = user.UserName
        };

        return Ok(profile);
    }


    [Authorize]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateUser(int id, UserUpdateDto dto)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (currentUserId != id.ToString())
            return Forbid();

        var user = _context.Users.Find(id);
        if (user == null)
            return NotFound(new { message = "User not found." });

        user.Name = dto.Name;
        user.BirthDate = dto.BirthDate.ToUniversalTime();

        _context.SaveChanges();

        return NoContent(); 
    }


    [Authorize]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteUser(int id)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (currentUserId != id.ToString())
            return Forbid();

        var user = _context.Users.Find(id);
        if (user == null)
            return NotFound(new { message = "User not found." });

        _context.Users.Remove(user);
        _context.SaveChanges();

        return NoContent();
    }


    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Login(UserLoginDto dto)
    {
        var user = _context.Users.SingleOrDefault(u => u.Email == dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            return Unauthorized(new { message = "Invalid email or password." });

        var token = GenerateJwtToken(user);
        return Ok(new { token = token, message = "Login successful!" });
    }


    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            return Unauthorized(new { message = "Invalid token payload." });

        var user = _context.Users.Find(userId);
        if (user == null)
            return NotFound(new { message = "User not found." });

        var response = new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            BirthDate = user.BirthDate,
            UserName = user.UserName,
            Email = user.Email
        };

        return Ok(response);
    }


    private string GenerateJwtToken(User user)
    {
        var jwtSecret = _configuration["JwtSettings:Secret"] ?? "TemporaryKeySoEFCoreMigrationDoesNotBreak!";
        var validIssuer = _configuration["JwtSettings:Issuer"];
        var validAudience = _configuration["JwtSettings:Audience"];

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("name", user.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: validIssuer,
            audience: validAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
