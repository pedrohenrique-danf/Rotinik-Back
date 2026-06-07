using Microsoft.EntityFrameworkCore;
using Rotinik.Core.Exceptions;
using Rotinik.Core.Data;
using Rotinik.Features.Routines.DTOs;

namespace Rotinik.Features.Routines;

public class RoutineService
{
    private readonly AppDbContext _context;

    public RoutineService(AppDbContext context)
    {
        _context = context;
    }

    private async Task<Routine> GetRoutineAndVerifyAccessAsync(int id, int currentUserId, string action, bool isAdmin = false)
    {
        var routine = await _context.Routines
            .Include(r => r.Tasks)
            .SingleOrDefaultAsync(r => r.Id == id);

        if (routine == null)
            throw new NotFoundException("Routine not found.");

        if (routine.IsDefault)
        {
            if (!isAdmin)
                throw new ForbiddenException("Forbidden: Only administrators can access global templates.");
            return routine;
        }

        if (routine.UserId != currentUserId && !isAdmin)
            throw new ForbiddenException($"Forbidden: You can only {action} your own routines.");

        return routine;
    }

    public async Task<RoutineResponseDto> GetRoutineByIdAsync(int id, int currentUserId, bool isAdmin = false)
    {
        var routine = await GetRoutineAndVerifyAccessAsync(id, currentUserId, "view", isAdmin);
        return routine.ToResponseDto();
    }

    public async Task<RoutineResponseDto> CreateRoutineAsync(int currentUserId, RoutineCreateDto dto)
    {
        var routine = new Routine
        {
            Title = dto.Title,
            Description = dto.Description,
            Category = dto.Category,
            Frequency = dto.Frequency,
            IsDefault = false,
            UserId = currentUserId
        };

        _context.Routines.Add(routine);
        await _context.SaveChangesAsync();

        return routine.ToResponseDto();
    }

    public async Task UpdateRoutineAsync(int id, int currentUserId, RoutineUpdateDto dto)
    {
        var routine = await GetRoutineAndVerifyAccessAsync(id, currentUserId, "update");

        if (!string.IsNullOrWhiteSpace(dto.Title))
            routine.Title = dto.Title;

        if (!string.IsNullOrWhiteSpace(dto.Category))
            routine.Category = dto.Category;

        if (dto.Description != null)
            routine.Description = dto.Description;

        if (!string.IsNullOrWhiteSpace(dto.Frequency))
            routine.Frequency = dto.Frequency;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteRoutineAsync(int id, int currentUserId)
    {
        var routine = await GetRoutineAndVerifyAccessAsync(id, currentUserId, "delete");

        _context.Routines.Remove(routine);
        await _context.SaveChangesAsync();
    }

    public async Task<List<RoutineResponseDto>> GetUserRoutinesAsync(int currentUserId)
    {
        var routines = await _context.Routines
            .AsNoTracking()
            .Include(r => r.Tasks)
            .Where(r => r.UserId == currentUserId)
            .ToListAsync();

        return routines.Select(r => r.ToResponseDto()).ToList();
    }

    public async Task<List<RoutineResponseDto>> GetTemplatesAsync()
    {
        var templates = await _context.Routines
            .AsNoTracking()
            .Include(r => r.Tasks)
            .Where(r => r.IsDefault)
            .ToListAsync();

        return templates.Select(r => r.ToResponseDto()).ToList();
    }

    public async Task<RoutineResponseDto> CloneTemplateAsync(int templateId, int userId)
    {
        var template = await _context.Routines
            .Include(r => r.Tasks)
            .FirstOrDefaultAsync(r => r.Id == templateId && r.IsDefault);

        if (template == null)
            throw new NotFoundException("Template not found.");

        var clonedRoutine = new Routine
        {
            Title = template.Title,
            Description = template.Description,
            Category = template.Category,
            Frequency = template.Frequency,
            IsDefault = false,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var task in template.Tasks)
        {
            clonedRoutine.Tasks.Add(new Tasks.TaskItem
            {
                Title = task.Title,
                Description = task.Description,
                Frequency = task.Frequency,
                Priority = task.Priority,
                IsCompleted = false,
                XpReward = task.XpReward,
                CoinReward = task.CoinReward,
                Order = task.Order,
                EstimatedMinutes = task.EstimatedMinutes
            });
        }

        _context.Routines.Add(clonedRoutine);
        await _context.SaveChangesAsync();

        return clonedRoutine.ToResponseDto();
    }

    public async Task<List<RoutineResponseDto>> AdminListAllRoutinesAsync()
    {
        var routines = await _context.Routines
            .AsNoTracking()
            .Include(r => r.Tasks)
            .OrderByDescending(r => r.IsDefault)
            .ThenBy(r => r.Title)
            .ToListAsync();

        return routines.Select(r => r.ToResponseDto()).ToList();
    }

    public async Task<RoutineResponseDto> AdminCreateTemplateAsync(AdminRoutineCreateDto dto)
    {
        var template = new Routine
        {
            Title = dto.Title,
            Description = dto.Description,
            Category = dto.Category,
            Frequency = dto.Frequency,
            IsDefault = true,
            UserId = null,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var t in dto.Tasks)
        {
            template.Tasks.Add(new Tasks.TaskItem
            {
                Title = t.Title,
                Description = t.Description ?? string.Empty,
                Frequency = t.Frequency,
                Priority = MapImportance(t.Importance),
                IsCompleted = false,
                XpReward = t.XpReward > 0 ? t.XpReward : 10,
                CoinReward = t.CoinReward > 0 ? t.CoinReward : 5,
                EstimatedMinutes = t.EstimatedMinutes > 0 ? t.EstimatedMinutes : 30
            });
        }

        _context.Routines.Add(template);
        await _context.SaveChangesAsync();

        return template.ToResponseDto();
    }

    public async Task AdminUpdateTemplateAsync(int id, RoutineUpdateDto dto)
    {
        var routine = await _context.Routines.FirstOrDefaultAsync(r => r.Id == id);
        if (routine == null)
            throw new NotFoundException("Routine template not found.");

        if (!string.IsNullOrWhiteSpace(dto.Title))
            routine.Title = dto.Title;

        if (!string.IsNullOrWhiteSpace(dto.Category))
            routine.Category = dto.Category;

        if (dto.Description != null)
            routine.Description = dto.Description;

        if (!string.IsNullOrWhiteSpace(dto.Frequency))
            routine.Frequency = dto.Frequency;

        await _context.SaveChangesAsync();
    }

    public async Task AdminDeleteTemplateAsync(int id)
    {
        var routine = await _context.Routines.FirstOrDefaultAsync(r => r.Id == id);
        if (routine == null)
            throw new NotFoundException("Routine template not found.");

        _context.Routines.Remove(routine);
        await _context.SaveChangesAsync();
    }

    private static Tasks.TaskPriority MapImportance(string? importance) => importance?.ToLower() switch
    {
        "baixa" => Tasks.TaskPriority.Low,
        "alta" => Tasks.TaskPriority.Important,
        "critica" => Tasks.TaskPriority.Urgent,
        _ => Tasks.TaskPriority.Moderate
    };
}