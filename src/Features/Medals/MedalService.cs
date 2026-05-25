using Microsoft.EntityFrameworkCore;
using Rotinik.Core.Data;
using Rotinik.Features.Medals.DTOs;

namespace Rotinik.Features.Medals;

public class MedalService
{
    private readonly AppDbContext _context;

    public MedalService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserMedalResponseDto>> GetUserMedalsAsync(int userId)
    {
        return await _context.Set<UserMedal>()
            .AsNoTracking()
            .Where(um => um.UserId == userId)
            .OrderByDescending(um => um.AchievedAt)
            .MapToUserMedalDto()
            .ToListAsync();
    }

    public async Task<List<MedalResponseDto>> GetAllMedalsAsync()
    {
        return await _context.Medals
            .AsNoTracking()
            .OrderBy(m => m.PointsThreshold)
            .MapToMedalDto()
            .ToListAsync();
    }

    public async Task<MedalProgressDto> GetNextMedalProgressAsync(int currentUserPoints)
    {
        var nextMedal = await _context.Medals
            .AsNoTracking()
            .Where(m => m.PointsThreshold > currentUserPoints)
            .OrderBy(m => m.PointsThreshold)
            .MapToMedalDto()
            .FirstOrDefaultAsync();

        return new MedalProgressDto
        {
            NextMedal = nextMedal,
            PointsNeeded = nextMedal != null ? nextMedal.PointsThreshold - currentUserPoints : 0
        };
    }

    public async Task<List<MedalResponseDto>> CheckAndAwardMedalsAsync(int userId, int currentUserPoints)
    {
        var existingMedalIds = await _context.Set<UserMedal>()
            .Where(um => um.UserId == userId)
            .Select(um => um.MedalId)
            .ToListAsync();

        var eligibleMedals = await _context.Medals
            .Where(m => m.PointsThreshold <= currentUserPoints && !existingMedalIds.Contains(m.Id))
            .ToListAsync();

        if (!eligibleMedals.Any())
            return new List<MedalResponseDto>();

        var userMedalsToAdd = eligibleMedals.Select(m => new UserMedal
        {
            UserId = userId,
            MedalId = m.Id
        });

        await _context.Set<UserMedal>().AddRangeAsync(userMedalsToAdd);
        await _context.SaveChangesAsync();

        return eligibleMedals.Select(m => new MedalResponseDto
        {
            Id = m.Id,
            Name = m.Name,
            Description = m.Description,
            IconUrl = m.IconUrl,
            PointsThreshold = m.PointsThreshold
        }).ToList();
    }
}