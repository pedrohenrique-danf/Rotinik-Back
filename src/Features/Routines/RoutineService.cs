using Microsoft.EntityFrameworkCore;
using Rotinik.Core.Exceptions;
using Rotinik.Core.Data;
using Rotinik.Features.Routines.DTO;

namespace Rotinik.Features.Routines;

public class RoutineService
{
    private readonly AppDbContext _context;

    public RoutineService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RoutineResponseDto> CreateRoutineAsync(int currentUserId, RoutineCreateDto dto)
    {
        var user = await _context.Users.FindAsync(currentUserId);
        if (user == null)
            throw new NotFoundException("User not found.");

        var routine = new Routine
        {
            Title = dto.Title,
            Category = dto.Category,
            IdUser = user
        };

        await _context.Routines.AddAsync(routine);
        await _context.SaveChangesAsync();
        
        return new RoutineResponseDto
        {
            Id = routine.Id,
            Title = routine.Title,
            Category = routine.Category
        };
    }

    public async Task UpdateRoutineAsync(int id, int currentUserId, RoutineUpdateDto dto)
    {
        var routine = await _context.Routines.Include(r => r.IdUser).SingleOrDefaultAsync(r => r.Id == id);
        if (routine == null)
            throw new NotFoundException("Routine not found.");

        if (routine.IdUser.Id != currentUserId)
            throw new ForbiddenException("Forbidden: You can only update your own routines.");

        if (!string.IsNullOrWhiteSpace(dto.Title))
            routine.Title = dto.Title;

        if (!string.IsNullOrWhiteSpace(dto.Category))
            routine.Category = dto.Category;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteRoutineAsync(int id, int currentUserId)
    {
        var routine = await _context.Routines.Include(r => r.IdUser).SingleOrDefaultAsync(r => r.Id == id);
        if (routine == null)
            throw new NotFoundException("Routine not found.");

        if (routine.IdUser.Id != currentUserId)
            throw new ForbiddenException("Forbidden: You can only delete your own routines.");

        _context.Routines.Remove(routine);
        await _context.SaveChangesAsync();
    }

    public async Task<List<RoutineResponseDto>> GetUserRoutinesAsync(int currentUserId)
    {
        return await _context.Routines
            .Where(r => r.IdUser.Id == currentUserId)
            .Select(r => new RoutineResponseDto
            {
                Id = r.Id,
                Title = r.Title,
                Category = r.Category
            })
            .ToListAsync();
    }
}