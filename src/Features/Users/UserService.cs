using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Rotinik.Core.Exceptions;
using Rotinik.Core.Data;
using Rotinik.Features.Users.DTOs;
using Rotinik.Features.Users.Auth;
using Rotinik.Features.Routines;
using Rotinik.Core.Extensions;

namespace Rotinik.Features.Users;

public class UserService
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;
    private readonly PasswordHasher _passwordHasher;
    private readonly IMemoryCache _cache;

    public UserService(
        AppDbContext context, 
        TokenService tokenService, 
        PasswordHasher passwordHasher, 
        IMemoryCache cache)
    {
        _context = context;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _cache = cache;
    }

    private async Task<User> GetUserOrThrowAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found.");

        return user;
    }

    public async Task<TokenDto> LoginAsync(UserLoginDto dto)
    {
        var user = await _context.Users
            .Include(u => u.RefreshTokens)
            .SingleOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || !_passwordHasher.VerifyPassword(dto.Password, user.Password))
            throw new UnauthorizedException("Invalid email or password.");

        var tokenPair = GenerateAndAssignTokens(user);
        await _context.SaveChangesAsync();

        return tokenPair;
    }

    public async Task<TokenDto> RefreshTokenAsync(TokenDto dto)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(dto.AccessToken);
        var userId = principal.GetCurrentUserId();

        var user = await _context.Users
            .Include(u => u.RefreshTokens)
            .SingleOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new UnauthorizedException("Invalid user.");

        var incomingToken = dto.RefreshToken;

        var activeSession = user.RefreshTokens.FirstOrDefault(rt =>
            rt.Token == incomingToken && rt.ExpiryTime > DateTime.UtcNow);

        if (activeSession == null)
            throw new UnauthorizedException("Invalid refresh token.");

        user.RefreshTokens.Remove(activeSession);

        var tokenPair = GenerateAndAssignTokens(user);
        await _context.SaveChangesAsync();

        return tokenPair;
    }

    private TokenDto GenerateAndAssignTokens(User user)
    {
        var accessToken = _tokenService.GenerateJwtToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        var expiredTokens = user.RefreshTokens.Where(rt => rt.ExpiryTime <= DateTime.UtcNow).ToList();
        foreach (var token in expiredTokens)
        {
            user.RefreshTokens.Remove(token);
        }

        user.RefreshTokens.Add(new UserRefreshToken
        {
            Token = refreshToken,
            ExpiryTime = DateTime.UtcNow.AddDays(7)
        });

        return new TokenDto { AccessToken = accessToken, RefreshToken = refreshToken };
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
            BirthDate = dto.BirthDate,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var defaultRoutine = new Routine
        {
            Title = "Inbox",
            Category = "System",
            IsDefault = true,
            UserId = user.Id
        };
        
        await _context.Routines.AddAsync(defaultRoutine);
        await _context.SaveChangesAsync();
    }

    public async Task<UserProfileDto?> GetPublicProfileAsync(string username)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);
        if (user == null)
            throw new NotFoundException("User not found.");

        var rankPosition = await CalculateUserRankAsync(user.Xp);

        return new UserProfileDto
        {
            Name = user.Name,
            UserName = user.UserName,
            Xp = user.Xp,
            IsPremium = user.IsPremium,
            RankPosition = rankPosition
        };
    }

    public async Task<UserResponseDto?> GetCurrentUserAsync(int userId)
    {
        var user = await GetUserOrThrowAsync(userId);
        var rankPosition = await CalculateUserRankAsync(user.Xp);

        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            UserName = user.UserName,
            Email = user.Email,
            BirthDate = user.BirthDate,
            Xp = user.Xp,
            Coins = user.Coins,
            IsPremium = user.IsPremium,
            RankPosition = rankPosition
        };
    }

    private async Task<int> CalculateUserRankAsync(int userXp)
    {
        var cacheKey = $"UserRank_{userXp}";
        
        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return await _context.Users.CountAsync(u => u.Xp > userXp) + 1;
        });
    }

    public async Task UpdateUserAsync(int id, int currentUserId, UserUpdateDto dto)
    {
        if (currentUserId != id)
            throw new ForbiddenException("Forbidden: You can only update your own profile.");

        var user = await GetUserOrThrowAsync(id);

        user.Name = dto.Name;
        user.BirthDate = dto.BirthDate; // Removed .ToUniversalTime()

        if (!string.IsNullOrEmpty(dto.Password))
        {
            user.Password = _passwordHasher.HashPassword(dto.Password);
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int id, int currentUserId)
    {
        if (currentUserId != id)
            throw new ForbiddenException("Forbidden: You can only delete your own profile.");

        var user = await GetUserOrThrowAsync(id);

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task ActivatePremiumAsync(int currentUserId)
    {
        var user = await GetUserOrThrowAsync(currentUserId);

        if (user.IsPremium)
            throw new ConflictException("Your account is already Premium.");

        user.IsPremium = true;
        await _context.SaveChangesAsync();
    }

    public async Task<List<UserRankDto>> GetTopRankedUsersAsync(int limit = 100)
    {
        var topUsers = await _context.Users
            .AsNoTracking()
            .OrderByDescending(u => u.Xp)
            .Take(limit)
            .Select(u => new
            {
                u.UserName,
                u.Xp,
                u.IsPremium
            })
            .ToListAsync();

        return topUsers.Select((u, index) => new UserRankDto
        {
            RankPosition = index + 1,
            UserName = u.UserName,
            Xp = u.Xp,
            IsPremium = u.IsPremium
        }).ToList();
    }
}