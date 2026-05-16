using Microsoft.IdentityModel.Tokens;
using Rotinik.Data;
using Rotinik.DTOs.User;
using Rotinik.Models;
using Rotinik.Core.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;

namespace Rotinik.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public UserService(AppDbContext context, IConfiguration configuration, IMapper mapper)
    {
        _context = context;
        _configuration = configuration;
        _mapper = mapper;
    }

    public void CreateUser(UserRegistrationDto dto)
    {
        if (_context.Users.Any(u => u.UserName == dto.UserName))
            throw new ConflictException("UserName in use.");

        if (_context.Users.Any(u => u.Email == dto.Email))
            throw new ConflictException("Email in use.");

        var user = _mapper.Map<User>(dto);
        
        user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);  

        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public UserProfileDto GetPublicProfile(string username)
    {
        var user = _context.Users.SingleOrDefault(u => u.UserName == username);
        if (user == null)  
            throw new NotFoundException("User not found.");

        return _mapper.Map<UserProfileDto>(user);
    }

    public void UpdateUser(int id, int currentUserId, UserUpdateDto dto)
    {
        if (currentUserId != id)
            throw new ForbiddenException("Forbidden: You can only update your own profile.");

        var user = _context.Users.Find(id);
        if (user == null)
            throw new NotFoundException("User not found.");

        user.Name = dto.Name;
        user.BirthDate = dto.BirthDate.ToUniversalTime();

        _context.SaveChanges();
    }

    public void DeleteUser(int id, int currentUserId)
    {
        if (currentUserId != id)
            throw new ForbiddenException("Forbidden: You can only delete your own profile.");

        var user = _context.Users.Find(id);
        if (user == null)
            throw new NotFoundException("User not found.");

        _context.Users.Remove(user);
        _context.SaveChanges();
    }

    public string Login(UserLoginDto dto)
    {
        var user = _context.Users.SingleOrDefault(u => u.Email == dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            throw new UnauthorizedAccessException("Invalid email or password."); // Middleware já mapeia isso para 401

        return GenerateJwtToken(user);
    }

    public UserResponseDto GetCurrentUser(int userId)
    {
        var user = _context.Users.Find(userId);
        if (user == null)  
            throw new NotFoundException("User not found.");

        return _mapper.Map<UserResponseDto>(user);
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