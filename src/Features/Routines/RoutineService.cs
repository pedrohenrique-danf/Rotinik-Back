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

    private async Task<Routine> GetRoutineAndVerifyAccessAsync(int id, int currentUserId, string action)
    {
        var routine = await _context.Routines
            .Include(r => r.Tasks)
            .SingleOrDefaultAsync(r => r.Id == id);

        if (routine == null)
            throw new NotFoundException("Routine not found.");

        if (routine.UserId != currentUserId)
            throw new ForbiddenException($"Forbidden: You can only {action} your own routines.");

        if (routine.IsDefault)
            throw new ForbiddenException($"Forbidden: You cannot {action} the system's default routine.");

        return routine;
    }

    public async Task<RoutineResponseDto> GetRoutineByIdAsync(int id, int currentUserId)
    {
        var routine = await GetRoutineAndVerifyAccessAsync(id, currentUserId, "view");
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
}