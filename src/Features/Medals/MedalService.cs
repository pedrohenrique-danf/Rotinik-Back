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
            .OrderBy(m => m.TriggerType)
            .ThenBy(m => m.TargetValue)
            .MapToMedalDto()
            .ToListAsync();
    }

    public async Task<MedalProgressDto> GetNextMedalProgressAsync(int currentValue, MedalTriggerType triggerType)
    {
        var nextMedal = await _context.Medals
            .AsNoTracking()
            .Where(m => m.TriggerType == triggerType && m.TargetValue > currentValue)
            .OrderBy(m => m.TargetValue)
            .MapToMedalDto()
            .FirstOrDefaultAsync();

        return new MedalProgressDto
        {
            NextMedal = nextMedal,
            TargetNeeded = nextMedal != null ? nextMedal.TargetValue - currentValue : 0
        };
    }

    public async Task<List<MedalResponseDto>> EvaluateMedalsAsync(int userId, MedalTriggerType trigger)
    {
        var existingMedalIds = await _context.Set<UserMedal>()
            .Where(um => um.UserId == userId)
            .Select(um => um.MedalId)
            .ToListAsync();

        var candidateMedals = await _context.Medals
            .AsNoTracking()
            .Where(m => m.TriggerType == trigger && !existingMedalIds.Contains(m.Id))
            .ToListAsync();

        if (!candidateMedals.Any())
            return new List<MedalResponseDto>(); 

        var medalsWon = new List<Medal>();

        switch (trigger)
        {
            case MedalTriggerType.TasksCompleted:
                var completedTasksCount = await _context.Tasks
                    .CountAsync(t => t.Routine.UserId == userId && t.IsCompleted); 
                
                medalsWon.AddRange(candidateMedals.Where(m => completedTasksCount >= m.TargetValue));
                break;

            case MedalTriggerType.RoutineStreak:
                var currentStreak = await GetUserCurrentStreakAsync(userId);
                medalsWon.AddRange(candidateMedals.Where(m => currentStreak >= m.TargetValue));
                break;

            case MedalTriggerType.TotalPoints:
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    medalsWon.AddRange(candidateMedals.Where(m => user.Points >= m.TargetValue));
                }
                break;
        }

        if (medalsWon.Any())
        {
            var userMedalsToAdd = medalsWon.Select(m => new UserMedal
            {
                UserId = userId,
                MedalId = m.Id,
                AchievedAt = DateTime.UtcNow
            });

            await _context.Set<UserMedal>().AddRangeAsync(userMedalsToAdd);
            await _context.SaveChangesAsync();
        }

        return medalsWon.AsQueryable().MapToMedalDto().ToList();
    }

    private async Task<int> GetUserCurrentStreakAsync(int userId)
    {
        return await Task.FromResult(0);
    }
}