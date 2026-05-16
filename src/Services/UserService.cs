using Microsoft.IdentityModel.Tokens;
using Rotinik.Data;
using Rotinik.DTOs.User;
using Rotinik.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace Rotinik.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public UserService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public void CreateUser(UserRegistrationDto dto)
    {
        if (dto.BirthDate > DateTime.UtcNow)
            throw new ArgumentException("Birth date cannot be in the future.");

        if (_context.Users.Any(u => u.UserName == dto.UserName))
            throw new InvalidOperationException("UserName in use.");

        if (_context.Users.Any(u => u.Email == dto.Email))
            throw new InvalidOperationException("Email in use.");

        var passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$";
        if (!Regex.IsMatch(dto.Password, passwordPattern))
            throw new ArgumentException("Password requirements: minimum 8 characters, 1 uppercase, 1 lowercase, 1 number, and 1 symbol.");

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
    }

    public UserProfileDto? GetPublicProfile(string username)
    {
        var user = _context.Users.SingleOrDefault(u => u.UserName == username);
        if (user == null) return null;

        return new UserProfileDto
        {
            Name = user.Name,
            UserName = user.UserName
        };
    }

    public void UpdateUser(int id, int currentUserId, UserUpdateDto dto)
    {
        if (currentUserId != id)
            throw new UnauthorizedAccessException("Forbidden");

        var user = _context.Users.Find(id);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        user.Name = dto.Name;
        user.BirthDate = dto.BirthDate.ToUniversalTime();

        // Se quiser atualizar a senha também, adicione a lógica de hash aqui

        _context.SaveChanges();
    }

    public void DeleteUser(int id, int currentUserId)
    {
        if (currentUserId != id)
            throw new UnauthorizedAccessException("Forbidden");

        var user = _context.Users.Find(id);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        _context.Users.Remove(user);
        _context.SaveChanges();
    }

    public string? Login(UserLoginDto dto)
    {
        var user = _context.Users.SingleOrDefault(u => u.Email == dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            return null; // Credenciais inválidas

        return GenerateJwtToken(user);
    }

    public UserResponseDto? GetCurrentUser(int userId)
    {
        var user = _context.Users.Find(userId);
        if (user == null) return null;

        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            BirthDate = user.BirthDate,
            UserName = user.UserName,
            Email = user.Email
        };
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