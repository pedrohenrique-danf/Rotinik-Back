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

            case MedalTriggerType.RoutinesCompleted:
                var completedRoutinesCount = await _context.DailyUserSummaries
                    .Where(d => d.UserId == userId)
                    .SumAsync(d => d.RoutinesCompleted);
                
                medalsWon.AddRange(candidateMedals.Where(m => completedRoutinesCount >= m.TargetValue));
                break;

            case MedalTriggerType.TotalPoints:
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    medalsWon.AddRange(candidateMedals.Where(m => user.Points >= m.TargetValue));
                }
                break;

            case MedalTriggerType.PremiumPurchased:
                var userPremium = await _context.Users.FindAsync(userId);
                if (userPremium != null && userPremium.IsPremium)
                {
                    medalsWon.AddRange(candidateMedals.Where(m => m.TargetValue == 1));
                }
                break;

            case MedalTriggerType.ShopItemPurchased:
                var purchasedItemsCount = await _context.Set<Rotinik.Features.Shop.UserShopItem>()
                    .CountAsync(ui => ui.UserId == userId);
                
                medalsWon.AddRange(candidateMedals.Where(m => purchasedItemsCount >= m.TargetValue));
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

    public async Task<MedalResponseDto> CreateMedalAsync(MedalCreateDto dto)
    {
        var medal = new Medal
        {
            Name = dto.Name,
            Description = dto.Description,
            IconUrl = dto.IconUrl,
            TriggerType = dto.TriggerType,
            TargetValue = dto.TargetValue
        };

        await _context.Medals.AddAsync(medal);
        await _context.SaveChangesAsync();

        return MapToResponse(medal);
    }

    public async Task UpdateMedalAsync(int id, MedalUpdateDto dto)
    {
        var medal = await _context.Medals.FindAsync(id);
        if (medal == null)
            throw new Core.Exceptions.NotFoundException("Medal not found.");

        if (dto.Name != null) medal.Name = dto.Name;
        if (dto.Description != null) medal.Description = dto.Description;
        if (dto.IconUrl != null) medal.IconUrl = dto.IconUrl;
        if (dto.TriggerType.HasValue) medal.TriggerType = dto.TriggerType.Value;
        if (dto.TargetValue.HasValue) medal.TargetValue = dto.TargetValue.Value;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteMedalAsync(int id)
    {
        var medal = await _context.Medals.FindAsync(id);
        if (medal == null)
            throw new Core.Exceptions.NotFoundException("Medal not found.");

        // Clear associated UserMedals records
        var userMedals = _context.Set<UserMedal>().Where(um => um.MedalId == id);
        _context.Set<UserMedal>().RemoveRange(userMedals);

        _context.Medals.Remove(medal);
        await _context.SaveChangesAsync();
    }

    public async Task EquipMedalsAsync(int userId, MedalEquipDto dto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new Core.Exceptions.NotFoundException("User not found.");

        int maxSlots = user.IsPremium ? 6 : 3;

        if (dto.MedalIds.Count > maxSlots)
            throw new ArgumentException($"You can only equip up to {maxSlots} medals.");

        // Obter as medalhas atuais do usuário
        var userMedals = await _context.Set<UserMedal>()
            .Where(um => um.UserId == userId)
            .ToListAsync();

        // Validar se o usuário possui todas as medalhas enviadas
        var invalidIds = dto.MedalIds.Except(userMedals.Select(um => um.MedalId)).ToList();
        if (invalidIds.Any())
            throw new ArgumentException("You do not own all the specified medals.");

        // Atualizar IsEquipped
        foreach (var um in userMedals)
        {
            um.IsEquipped = dto.MedalIds.Contains(um.MedalId);
        }

        await _context.SaveChangesAsync();
    }

    private static MedalResponseDto MapToResponse(Medal m)
    {
        return new MedalResponseDto
        {
            Id = m.Id,
            Name = m.Name,
            Description = m.Description,
            IconUrl = m.IconUrl,
            TriggerType = m.TriggerType,
            TargetValue = m.TargetValue
        };
    }
}