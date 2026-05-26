using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotinik.Core.Extensions;
using Rotinik.Core.Data;
using Rotinik.Features.Routines.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Rotinik.Features.Routines;

[Authorize]
[Route("api/routine")]
[ApiController]
public class RoutineController : ControllerBase
{
    private readonly RoutineService _routineService;
    private readonly AppDbContext _context;

    public RoutineController(RoutineService routineService, AppDbContext context)
    {
        _routineService = routineService;
        _context = context;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateRoutine(RoutineCreateDto dto)
    {
        var currentUserId = User.GetCurrentUserId();

        var result = await _routineService.CreateRoutineAsync(currentUserId, dto);

        return CreatedAtAction(nameof(GetUserRoutines), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRoutine(int id, RoutineUpdateDto dto)
    {
        var currentUserId = User.GetCurrentUserId();
        await _routineService.UpdateRoutineAsync(id, currentUserId, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRoutine(int id)
    {
        var currentUserId = User.GetCurrentUserId();
        await _routineService.DeleteRoutineAsync(id, currentUserId);
        return NoContent();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserRoutines()
    {
        var currentUserId = User.GetCurrentUserId();
        var routines = await _routineService.GetUserRoutinesAsync(currentUserId);

        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == currentUserId);
        if (user == null) return Unauthorized();

        // Compute level from points (simple formula: level = sqrt(points/100))
        var level = Math.Max(1, (int)Math.Floor(Math.Sqrt(user.Points / 100.0)) + 1);
        var nextLevelXp = (int)Math.Pow(level, 2) * 100;
        var prevLevelXp = (int)Math.Pow(level - 1, 2) * 100;
        var levelProgress = nextLevelXp > prevLevelXp
            ? (double)(user.Points - prevLevelXp) / (nextLevelXp - prevLevelXp)
            : 0;

        var snapshot = new
        {
            user = new
            {
                id = user.Id.ToString(),
                name = user.Name,
                userName = user.UserName,
                email = user.Email,
                level,
                currentXp = user.Points,
                totalXp = user.Points,
                coins = user.Coins,
                levelProgress,
                nextLevelXp
            },
            routines
        };

        return Ok(snapshot);
    }
}