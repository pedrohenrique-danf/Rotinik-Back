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
            .Include(um => um.Medal)
            .Where(um => um.UserId == userId)
            .OrderByDescending(um => um.AchievedAt)
            .Select(um => new UserMedalResponseDto
            {
                AchievedAt = um.AchievedAt,
                Medal = new MedalResponseDto
                {
                    Id = um.Medal.Id,
                    Name = um.Medal.Name,
                    Description = um.Medal.Description,
                    IconUrl = um.Medal.IconUrl,
                    PointsThreshold = um.Medal.PointsThreshold
                }
            })
            .ToListAsync();
    }

    public async Task CheckAndAwardMedalsAsync(int userId, int currentUserPoints)
    {
        var existingMedalIds = await _context.Set<UserMedal>()
            .Where(um => um.UserId == userId)
            .Select(um => um.MedalId)
            .ToListAsync();

        var eligibleMedals = await _context.Medals
            .Where(m => m.PointsThreshold <= currentUserPoints && !existingMedalIds.Contains(m.Id))
            .ToListAsync();

        if (!eligibleMedals.Any())
            return;

        var userMedalsToAdd = eligibleMedals.Select(m => new UserMedal
        {
            UserId = userId,
            MedalId = m.Id,
            AchievedAt = DateTime.UtcNow
        });

        await _context.Set<UserMedal>().AddRangeAsync(userMedalsToAdd);
        await _context.SaveChangesAsync();
    }
}