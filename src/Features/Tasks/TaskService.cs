using Microsoft.EntityFrameworkCore;
using Rotinik.Core.Exceptions;
using Rotinik.Core.Data;
using Rotinik.Features.Tasks.DTOs;
using Rotinik.Features.Medals;
using Rotinik.Features.Medals.DTOs;

namespace Rotinik.Features.Tasks;

public class TaskService
{
    private readonly AppDbContext _context;
    private readonly MedalService _medalService;

    public TaskService(AppDbContext context, MedalService medalService)
    {
        _context = context;
        _medalService = medalService;
    }

    private async Task VerifyRoutineOwnershipAsync(int routineId, int currentUserId)
    {
        var routine = await _context.Routines
            .AsNoTracking()
            .SingleOrDefaultAsync(r => r.Id == routineId);

        if (routine == null)
            throw new NotFoundException("Routine not found.");

        if (routine.UserId != currentUserId)
            throw new ForbiddenException("Forbidden: You can only modify tasks in your own routines.");
    }

    public async Task<TaskResponseDto> CreateTaskAsync(int routineId, int currentUserId, TaskCreateDto dto)
    {
        await VerifyRoutineOwnershipAsync(routineId, currentUserId);

        var task = new TaskItem
        {
            Title = dto.Title,
<<<<<<< HEAD
            Description = dto.Description ?? string.Empty,
            Frequency = dto.Frequency,
=======
            Description = dto.Description,
            ExecutionTime = dto.ExecutionTime,
>>>>>>> 8f6767e86801caf4e023b326a294d23f55ee89ed
            Priority = dto.Priority,
            IsCompleted = false,
            CompletedAt = null,
            XpReward = dto.XpReward > 0 ? dto.XpReward : 10,
            CoinReward = dto.CoinReward > 0 ? dto.CoinReward : 5,
            RoutineId = routineId
        };

        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        return MapToResponse(task);
    }

    public async Task UpdateTaskAsync(int routineId, int taskId, int currentUserId, TaskUpdateDto dto)
    {
        await VerifyRoutineOwnershipAsync(routineId, currentUserId);

        var task = await _context.Tasks.SingleOrDefaultAsync(t => t.Id == taskId && t.RoutineId == routineId);
        if (task == null) throw new NotFoundException("Task not found.");

        // Only Update Allowed Fields
        if (!string.IsNullOrWhiteSpace(dto.Title)) task.Title = dto.Title;
        if (dto.Description != null) task.Description = dto.Description;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteTaskAsync(int routineId, int taskId, int currentUserId)
    {
        await VerifyRoutineOwnershipAsync(routineId, currentUserId);

        var task = await _context.Tasks.SingleOrDefaultAsync(t => t.Id == taskId && t.RoutineId == routineId);
        if (task == null) throw new NotFoundException("Task not found.");

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
    }

    public async Task<List<MedalResponseDto>> ToggleTaskCompletionAsync(int routineId, int taskId, int currentUserId)
    {
        await VerifyRoutineOwnershipAsync(routineId, currentUserId);

        var task = await _context.Tasks.SingleOrDefaultAsync(t => t.Id == taskId && t.RoutineId == routineId);
        if (task == null) throw new NotFoundException("Task not found.");

        var user = await _context.Users.FindAsync(currentUserId);
        if (user == null) throw new NotFoundException("User not found.");

        var newlyUnlockedMedals = new List<MedalResponseDto>();

        if (!task.IsCompleted)
        {
            task.IsCompleted = true;
            task.CompletedAt = DateTime.UtcNow;
            
            user.Points += 10;
            user.Coins += 5;
            
            await _context.SaveChangesAsync();

            newlyUnlockedMedals.AddRange(await _medalService.EvaluateMedalsAsync(currentUserId, MedalTriggerType.TasksCompleted));
            newlyUnlockedMedals.AddRange(await _medalService.EvaluateMedalsAsync(currentUserId, MedalTriggerType.TotalPoints));
        }
        else
        {
            task.IsCompleted = false;
            task.CompletedAt = null;
            
            user.Points = Math.Max(0, user.Points - 10);
            user.Coins = Math.Max(0, user.Coins - 5);
            
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
<<<<<<< HEAD
=======
            ExecutionTime = task.ExecutionTime,
            Priority = task.Priority.ToString(),
>>>>>>> 8f6767e86801caf4e023b326a294d23f55ee89ed
            IsCompleted = task.IsCompleted,
            XpReward = task.XpReward,
            CoinReward = task.CoinReward,
            Order = task.Order,
            CompletedAt = task.CompletedAt,
            RoutineId = task.RoutineId
        };
    }

    public async Task<List<TaskResponseDto>> GetTasksByRoutineAsync(int routineId, int currentUserId)
    {
        await VerifyRoutineOwnershipAsync(routineId, currentUserId);

        return await _context.Tasks
            .Where(t => t.RoutineId == routineId)
            .OrderBy(t => t.IsCompleted)
            .ThenBy(t => t.Order)
            .Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
<<<<<<< HEAD
=======
                ExecutionTime = t.ExecutionTime,
                Priority = t.Priority.ToString(),
>>>>>>> 8f6767e86801caf4e023b326a294d23f55ee89ed
                IsCompleted = t.IsCompleted,
                XpReward = t.XpReward,
                CoinReward = t.CoinReward,
                Order = t.Order,
                CompletedAt = t.CompletedAt,
                RoutineId = t.RoutineId
            })
            .ToListAsync();
    }
}