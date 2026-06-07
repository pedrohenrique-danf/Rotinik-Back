using Microsoft.EntityFrameworkCore;
using Rotinik.Core.Exceptions;
using Rotinik.Core.Data;
using Rotinik.Features.Tasks.DTOs;
using Rotinik.Features.Medals;
using Rotinik.Features.Medals.DTOs;
using Rotinik.Features.Statistics;

namespace Rotinik.Features.Tasks;

public class TaskService
{
    private const int MaxDailyPoints = 500;
    private const int MaxDailyCoins = 100;
    private readonly AppDbContext _context;
    private readonly MedalService _medalService;

    public TaskService(AppDbContext context, MedalService medalService)
    {
        _context = context;
        _medalService = medalService;
    }

    private async Task VerifyRoutineOwnershipAsync(int routineId, int currentUserId, bool isAdmin = false)
    {
        var routine = await _context.Routines
            .AsNoTracking()
            .SingleOrDefaultAsync(r => r.Id == routineId);

        if (routine == null)
            throw new NotFoundException("Routine not found.");

        if (routine.IsDefault)
        {
            if (!isAdmin)
                throw new ForbiddenException("Forbidden: Only administrators can modify global templates.");
            return;
        }

        if (routine.UserId != currentUserId)
            throw new ForbiddenException("Forbidden: You can only modify tasks in your own routines.");
    }

    private (int xp, int coins) GetBaseRewardsForPriority(TaskPriority priority) => priority switch
    {
        TaskPriority.Low => (10, 5),
        TaskPriority.Moderate => (20, 10),
        TaskPriority.Important => (30, 15),
        TaskPriority.Urgent => (50, 25),
        _ => (10, 5)
    };

    public async Task<TaskResponseDto> CreateTaskAsync(int routineId, int currentUserId, TaskCreateDto dto, bool isAdmin = false)
    {
        await VerifyRoutineOwnershipAsync(routineId, currentUserId, isAdmin);

        var priority = MapImportance(dto.Importance);
        var (xp, coins) = GetBaseRewardsForPriority(priority);

        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description ?? string.Empty,
            Frequency = dto.Frequency,
            Priority = priority,
            DeadlineValue = dto.DeadlineValue,
            IsCompleted = false,
            CompletedAt = null,
            XpReward = xp,
            CoinReward = coins,
            RoutineId = routineId
        };

        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        return MapToResponse(task);
    }

    public async Task UpdateTaskAsync(int routineId, int taskId, int currentUserId, TaskUpdateDto dto, bool isAdmin = false)
    {
        await VerifyRoutineOwnershipAsync(routineId, currentUserId, isAdmin);

        var task = await _context.Tasks.SingleOrDefaultAsync(t => t.Id == taskId && t.RoutineId == routineId);
        if (task == null) throw new NotFoundException("Task not found.");

        if (!string.IsNullOrWhiteSpace(dto.Title)) task.Title = dto.Title;
        if (dto.Description != null) task.Description = dto.Description;
        if (dto.Frequency.HasValue) task.Frequency = dto.Frequency.Value;
        if (!string.IsNullOrEmpty(dto.Importance)) 
        {
            task.Priority = MapImportance(dto.Importance);
            var (xp, coins) = GetBaseRewardsForPriority(task.Priority);
            task.XpReward = xp;
            task.CoinReward = coins;
        }
        if (!string.IsNullOrWhiteSpace(dto.DeadlineValue)) task.DeadlineValue = dto.DeadlineValue;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteTaskAsync(int routineId, int taskId, int currentUserId, bool isAdmin = false)
    {
        await VerifyRoutineOwnershipAsync(routineId, currentUserId, isAdmin);

        var task = await _context.Tasks.SingleOrDefaultAsync(t => t.Id == taskId && t.RoutineId == routineId);
        if (task == null) throw new NotFoundException("Task not found.");

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
    }

    public async Task StartTaskAsync(int routineId, int taskId, int currentUserId, bool isAdmin = false)
    {
        await VerifyRoutineOwnershipAsync(routineId, currentUserId, isAdmin);

        var task = await _context.Tasks.SingleOrDefaultAsync(t => t.Id == taskId && t.RoutineId == routineId);
        if (task == null) throw new NotFoundException("Task not found.");
        if (task.IsCompleted) throw new BadRequestException("Task is already completed.");

        task.StartedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<List<MedalResponseDto>> ToggleTaskCompletionAsync(int routineId, int taskId, int currentUserId, bool isAdmin = false)
    {
        await VerifyRoutineOwnershipAsync(routineId, currentUserId, isAdmin);

        var task = await _context.Tasks.SingleOrDefaultAsync(t => t.Id == taskId && t.RoutineId == routineId);
        if (task == null) throw new NotFoundException("Task not found.");

        var user = await _context.Users.FindAsync(currentUserId);
        if (user == null) throw new NotFoundException("User not found.");

        var newlyUnlockedMedals = new List<MedalResponseDto>();

        if (!task.IsCompleted)
        {
            var today = DateTime.UtcNow.Date;
            var dailySummary = await _context.DailyUserSummaries
                .FirstOrDefaultAsync(d => d.UserId == currentUserId && d.Date == today);

            if (dailySummary == null)
            {
                dailySummary = new DailyUserSummary { UserId = currentUserId, Date = today };
                _context.DailyUserSummaries.Add(dailySummary);
            }

            int pointsToAward = task.XpReward;
            int coinsToAward = task.CoinReward;

            if (!string.IsNullOrWhiteSpace(task.DeadlineValue) && TimeOnly.TryParse(task.DeadlineValue, out var deadlineTime))
            {
                var localTime = TimeOnly.FromDateTime(DateTime.UtcNow.AddHours(-3));
                if (localTime <= deadlineTime)
                {
                    coinsToAward += (int)(coinsToAward * 0.5); // Bônus de 50%
                }
            }

            if (dailySummary.PointsEarned + pointsToAward > MaxDailyPoints)
                pointsToAward = Math.Max(0, MaxDailyPoints - dailySummary.PointsEarned);

            if (dailySummary.CoinsEarned + coinsToAward > MaxDailyCoins)
                coinsToAward = Math.Max(0, MaxDailyCoins - dailySummary.CoinsEarned);

            task.IsCompleted = true;
            task.CompletedAt = DateTime.UtcNow;

            _context.TaskHistories.Add(new TaskHistory
            {
                TaskId = task.Id,
                UserId = currentUserId,
                Status = TaskHistoryStatus.Completed,
                Date = DateTime.UtcNow,
                XpEarned = pointsToAward,
                CoinsEarned = coinsToAward
            });

            user.Points += pointsToAward;
            user.Coins += coinsToAward;
            
            dailySummary.PointsEarned += pointsToAward;
            dailySummary.CoinsEarned += coinsToAward;

            if (pointsToAward > 0)
            {
                _context.Set<WalletTransaction>().Add(new WalletTransaction
                {
                    UserId = currentUserId,
                    Amount = pointsToAward,
                    Currency = CurrencyType.Points,
                    Type = TransactionType.Earned,
                    Source = TransactionSource.Task,
                    Description = $"Concluiu: {task.Title}",
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (coinsToAward > 0)
            {
                _context.Set<WalletTransaction>().Add(new WalletTransaction
                {
                    UserId = currentUserId,
                    Amount = coinsToAward,
                    Currency = CurrencyType.Coins,
                    Type = TransactionType.Earned,
                    Source = TransactionSource.Task,
                    Description = $"Concluiu: {task.Title}",
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();

            newlyUnlockedMedals.AddRange(await _medalService.EvaluateMedalsAsync(currentUserId, MedalTriggerType.TasksCompleted));
            newlyUnlockedMedals.AddRange(await _medalService.EvaluateMedalsAsync(currentUserId, MedalTriggerType.TotalPoints));
        }
        else
        {
            task.IsCompleted = false;
            task.CompletedAt = null;
            task.StartedAt = null; 

            user.Points = Math.Max(0, user.Points - task.XpReward);
            user.Coins = Math.Max(0, user.Coins - task.CoinReward);

            var today = DateTime.UtcNow.Date;
            var dailySummary = await _context.DailyUserSummaries
                .FirstOrDefaultAsync(d => d.UserId == currentUserId && d.Date == today);
            
            if (dailySummary != null)
            {
                dailySummary.PointsEarned = Math.Max(0, dailySummary.PointsEarned - task.XpReward);
                dailySummary.CoinsEarned = Math.Max(0, dailySummary.CoinsEarned - task.CoinReward);
            }

            _context.Set<WalletTransaction>().Add(new WalletTransaction
            {
                UserId = currentUserId,
                Amount = task.XpReward,
                Currency = CurrencyType.Points,
                Type = TransactionType.Penalty, // Ou TransactionType.Lost
                Source = TransactionSource.Task,
                Description = $"Desmarcou: {task.Title}",
                CreatedAt = DateTime.UtcNow
            });

            _context.Set<WalletTransaction>().Add(new WalletTransaction
            {
                UserId = currentUserId,
                Amount = task.CoinReward,
                Currency = CurrencyType.Coins,
                Type = TransactionType.Penalty,
                Source = TransactionSource.Task,
                Description = $"Desmarcou: {task.Title}",
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }

    return newlyUnlockedMedals;
}

    private static TaskResponseDto MapToResponse(TaskItem task)
    {
        return new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            IsCompleted = task.IsCompleted,
            XpReward = task.XpReward,
            CoinReward = task.CoinReward,
            Order = task.Order,
            CompletedAt = task.CompletedAt,
            RoutineId = task.RoutineId,
            DeadlineValue = task.DeadlineValue,
            Importance = task.Priority switch
            {
                TaskPriority.Low => "baixa",
                TaskPriority.Moderate => "media",
                TaskPriority.Important => "alta",
                TaskPriority.Urgent => "critica",
                _ => "media"
            }
        };
    }

    public async Task<List<TaskResponseDto>> GetTasksByRoutineAsync(int routineId, int currentUserId, bool isAdmin = false)
    {
        await VerifyRoutineOwnershipAsync(routineId, currentUserId, isAdmin);

        return await _context.Tasks
            .Where(t => t.RoutineId == routineId)
            .OrderBy(t => t.IsCompleted)
            .ThenBy(t => t.Order)
            .Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IsCompleted = t.IsCompleted,
                XpReward = t.XpReward,
                CoinReward = t.CoinReward,
                Order = t.Order,
                CompletedAt = t.CompletedAt,
                RoutineId = t.RoutineId,
                DeadlineValue = t.DeadlineValue,
                Importance = t.Priority == TaskPriority.Low ? "baixa" :
                             t.Priority == TaskPriority.Moderate ? "media" :
                             t.Priority == TaskPriority.Important ? "alta" :
                             t.Priority == TaskPriority.Urgent ? "critica" : "media"
            })
            .ToListAsync();
    }

    private static TaskPriority MapImportance(string? importance) => importance?.ToLower() switch
    {
        "baixa" => TaskPriority.Low,
        "alta" => TaskPriority.Important,
        "critica" => TaskPriority.Urgent,
        _ => TaskPriority.Moderate
    };
}