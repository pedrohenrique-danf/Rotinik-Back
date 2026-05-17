using Microsoft.IdentityModel.Tokens;
using Rotinik.Data;
using Rotinik.DTOs.User;
using Rotinik.Models;
using Rotinik.Core.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Rotinik.Settings;

namespace Rotinik.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;

    public UserService(AppDbContext context, IMapper mapper, ITokenService tokenService)
    {
        _context = context;
        _mapper = mapper;
        _tokenService = tokenService;
    }

    public async Task CreateUserAsync(UserRegistrationDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.UserName == dto.UserName))
            throw new ConflictException("UserName in use.");

        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            throw new ConflictException("Email in use.");

        var user = _mapper.Map<User>(dto);
        
        user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);  

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task<UserProfileDto?> GetPublicProfileAsync(string username)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);
        if (user == null)  
            throw new NotFoundException("User not found.");

        return _mapper.Map<UserProfileDto>(user);
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

    public async Task<string?> LoginAsync(UserLoginDto dto)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return _tokenService.GenerateJwtToken(user);
    }

    public async Task<UserResponseDto?> GetCurrentUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)  
            throw new NotFoundException("User not found.");

        return _mapper.Map<UserResponseDto>(user);
    }
}