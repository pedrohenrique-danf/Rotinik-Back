using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Rotinik.Core.Exceptions;
using Rotinik.Data;
using Rotinik.DTOs.User;
using Rotinik.Models;

namespace Rotinik.Services;

public class UserService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly PasswordHasher _passwordHasher;

    public UserService(AppDbContext context, IMapper mapper, PasswordHasher passwordHasher)
    {
        _context = context;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
    }

    public async Task CreateUserAsync(UserRegistrationDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.UserName == dto.UserName))
            throw new ConflictException("UserName in use.");

        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            throw new ConflictException("Email in use.");

        var user = _mapper.Map<User>(dto);
        user.Password = _passwordHasher.HashPassword(dto.Password);  

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
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
            user.Password = _passwordHasher.HashPassword(dto.Password);

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

    public async Task<UserProfileDto?> GetPublicProfileAsync(string username)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);
        if (user == null)  
            throw new NotFoundException("User not found.");

        return _mapper.Map<UserProfileDto>(user);
    }

    public async Task<UserResponseDto?> GetCurrentUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)  
            throw new NotFoundException("User not found.");

        return _mapper.Map<UserResponseDto>(user);
    }
}