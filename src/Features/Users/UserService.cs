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
            .FirstOrDefaultAsync(u => u.Email == dto.Email || u.UserName == dto.Email);

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

        var rankPosition = await CalculateUserRankAsync(user.Points);

        return new UserProfileDto
        {
            Name = user.Name,
            UserName = user.UserName,
            Points = user.Points,
            IsPremium = user.IsPremium,
            RankPosition = rankPosition
        };
    }

    public async Task<UserResponseDto?> GetCurrentUserAsync(int userId)
    {
        var user = await GetUserOrThrowAsync(userId);
        var rankPosition = await CalculateUserRankAsync(user.Points);

        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            UserName = user.UserName,
            Email = user.Email,
            BirthDate = user.BirthDate,
            Points = user.Points,
            Coins = user.Coins,
            IsPremium = user.IsPremium,
            RankPosition = rankPosition,
            Role = user.Role,
            IsAdmin = user.IsAdmin
        };
    }

    private async Task<int> CalculateUserRankAsync(int userPoints)
    {
        var cacheKey = $"UserRank_{userPoints}";
        
        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return await _context.Users.CountAsync(u => u.Points > userPoints) + 1;
        });
    }

    public async Task UpdateUserAsync(int id, int currentUserId, UserUpdateDto dto)
    {
        if (currentUserId != id)
            throw new ForbiddenException("Forbidden: You can only update your own profile.");

        var user = await GetUserOrThrowAsync(id);

        user.Name = dto.Name;
        user.BirthDate = dto.BirthDate.ToUniversalTime();

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
            .OrderByDescending(u => u.Points)
            .Take(limit)
            .Select(u => new
            {
                u.UserName,
                u.Points,
                u.IsPremium
            })
            .ToListAsync();

        return topUsers.Select((u, index) => new UserRankDto
        {
            RankPosition = index + 1,
            UserName = u.UserName,
            Points = u.Points,
            IsPremium = u.IsPremium
        }).ToList();
    }

    public async Task<PaginatedResultDto<UserResponseDto>> ListUsersAdminAsync(string? search, int page, int pageSize)
    {
        var query = _context.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var cleanSearch = search.Trim().ToLower();
            query = query.Where(u => u.Name.ToLower().Contains(cleanSearch) || 
                                     u.UserName.ToLower().Contains(cleanSearch) || 
                                     u.Email.ToLower().Contains(cleanSearch));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(u => u.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = new List<UserResponseDto>();
        foreach (var u in items)
        {
            var rankPosition = await CalculateUserRankAsync(u.Points);
            dtos.Add(new UserResponseDto
            {
                Id = u.Id,
                Name = u.Name,
                UserName = u.UserName,
                Email = u.Email,
                BirthDate = u.BirthDate,
                Points = u.Points,
                Coins = u.Coins,
                IsPremium = u.IsPremium,
                RankPosition = rankPosition,
                Role = u.Role,
                IsAdmin = u.IsAdmin
            });
        }

        return new PaginatedResultDto<UserResponseDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task UpdateUserAdminAsync(int id, AdminUserUpdateDto dto)
    {
        var user = await GetUserOrThrowAsync(id);

        if (await _context.Users.AnyAsync(u => u.UserName == dto.UserName && u.Id != id))
            throw new ConflictException("UserName in use.");

        if (await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != id))
            throw new ConflictException("Email in use.");

        user.Name = dto.Name;
        user.Email = dto.Email;
        user.UserName = dto.UserName;
        user.Role = dto.Role;
        user.Points = dto.Points;
        user.Coins = dto.Coins;
        user.IsPremium = dto.IsPremium;

        await _context.SaveChangesAsync();
    }
}