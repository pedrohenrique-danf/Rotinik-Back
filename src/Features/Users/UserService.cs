using Microsoft.EntityFrameworkCore;
using Rotinik.Core.Exceptions;
using Rotinik.Core.Data;
using Rotinik.Features.Users.DTOs;
using System.Security.Claims;

namespace Rotinik.Features.Users;

public class UserService
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;

    public UserService(AppDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> LoginAsync(UserLoginDto dto)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == dto.Email);
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            throw new UnauthorizedException("Invalid email or password.");

        var accessToken = _tokenService.GenerateJwtToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        return new AuthResponseDto { AccessToken = accessToken, RefreshToken = refreshToken };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(dto.AccessToken);
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            throw new UnauthorizedException("Invalid token payload.");

        var user = await _context.Users.FindAsync(userId);

        if (user == null || user.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new UnauthorizedException("Invalid refresh token.");

        var newAccessToken = _tokenService.GenerateJwtToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _context.SaveChangesAsync();

        return new AuthResponseDto { AccessToken = newAccessToken, RefreshToken = newRefreshToken };
    }

    public async Task CreateUserAsync(UserRegistrationDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.UserName == dto.UserName))
            throw new ConflictException("UserName in use.");

        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            throw new ConflictException("Email in use.");

        var user = new User
        {
            Name = dto.Name,
            UserName = dto.UserName,
            Email = dto.Email,
            BirthDate = dto.BirthDate.ToUniversalTime(),
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<UserProfileDto?> GetPublicProfileAsync(string username)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);
        if (user == null) 
            throw new NotFoundException("User not found.");

        return new UserProfileDto
        {
            Name = user.Name,
            UserName = user.UserName
        };
    }

    public async Task<UserResponseDto?> GetCurrentUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) 
            throw new NotFoundException("User not found.");

        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            UserName = user.UserName,
            Email = user.Email,
            BirthDate = user.BirthDate
        };
    }

    public async Task UpdateUserAsync(int id, int currentUserId, UserUpdateDto dto)
    {
        if (currentUserId != id)
            throw new ForbiddenException("Forbidden: You can only update your own profile.");

        var user = await _context.Users.FindAsync(id);
        if (user == null) 
            throw new NotFoundException("User not found.");

        user.Name = dto.Name;
        user.BirthDate = dto.BirthDate.ToUniversalTime();

        if (!string.IsNullOrEmpty(dto.Password))
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int id, int currentUserId)
    {
        if (currentUserId != id)
            throw new ForbiddenException("Forbidden: You can only delete your own profile.");

        var user = await _context.Users.FindAsync(id);
        if (user == null) 
            throw new NotFoundException("User not found.");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task ActivatePremiumAsync(int currentUserId)
    {
        var user = await _context.Users.FindAsync(currentUserId);
        
        if (user == null)
            throw new NotFoundException("User not found.");

        if (user.isPremium)
            throw new ConflictException("Your account is already Premium.");

        user.isPremium = true;
        await _context.SaveChangesAsync();
    }
}