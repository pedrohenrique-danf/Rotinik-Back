using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotinik.Core.Extensions;
using Rotinik.Features.Routines.DTOs;

namespace Rotinik.Features.Routines;

[Authorize]
[Route("api/routine")]
[ApiController]
public class RoutineController : ControllerBase
{
    private readonly RoutineService _routineService;

    public RoutineController(RoutineService routineService)
    {
        _routineService = routineService;
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

        return CreatedAtAction(nameof(GetUserRoutines), new { id = result.Id }, new { data = result, message = "Routine created." });
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

        return Ok(new { data = routines });
    }
}