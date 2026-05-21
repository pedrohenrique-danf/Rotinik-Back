using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Rotinik.Core.Exceptions;
using Rotinik.Core.Data;

namespace Rotinik.Features.Routines;

public class RoutineService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public RoutineService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task CreateRoutineAsync(int currentUserId, RoutineCreateDto dto)
    {
        var user = await _context.Users.FindAsync(currentUserId);
        if (user == null)
            throw new NotFoundException("User not found.");

        var routine = _mapper.Map<Routine>(dto);
        routine.IdUser = user;

        await _context.Routines.AddAsync(routine);
        await _context.SaveChangesAsync();
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
}