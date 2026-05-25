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
            Frequency = dto.Frequency,
            Priority = dto.Priority,
            IsCompleted = false,
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

        if (!string.IsNullOrWhiteSpace(dto.Title)) task.Title = dto.Title;
        if (dto.Frequency.HasValue) task.Frequency = dto.Frequency.Value;
        if (dto.Priority.HasValue) task.Priority = dto.Priority.Value;

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
            user.Points += 10;
            user.Coins += 5;
            
            // Save state FIRST so the MedalService reads the updated points and completed tasks count
            await _context.SaveChangesAsync();

            // TRIGGER MEDAL EVALUATIONS
            newlyUnlockedMedals.AddRange(await _medalService.EvaluateMedalsAsync(currentUserId, MedalTriggerType.TasksCompleted));
            newlyUnlockedMedals.AddRange(await _medalService.EvaluateMedalsAsync(currentUserId, MedalTriggerType.TotalPoints));
        }
        else
        {
            task.IsCompleted = false;
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
            Frequency = task.Frequency.ToString(),
            Priority = task.Priority.ToString(),
            IsCompleted = task.IsCompleted,
            RoutineId = task.RoutineId
        };
    }

    public async Task<List<TaskResponseDto>> GetTasksByRoutineAsync(int routineId, int currentUserId)
    {
        await VerifyRoutineOwnershipAsync(routineId, currentUserId);

        return await _context.Tasks
            .Where(t => t.RoutineId == routineId)
            .OrderBy(t => t.IsCompleted)
            .Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Frequency = t.Frequency.ToString(),
                Priority = t.Priority.ToString(),
                IsCompleted = t.IsCompleted,
                RoutineId = t.RoutineId
            })
            .ToListAsync();
    }
}