using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotinik.DTOs.Routine;
using Rotinik.Services;
using System.Security.Claims;

namespace Rotinik.Controllers;

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
        var currentUserId = GetCurrentUserId();
        await _routineService.CreateRoutineAsync(currentUserId, dto);
        return StatusCode(201, new { message = "Routine created." });
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRoutine(int id, RoutineUpdateDto dto)
    {
        var currentUserId = GetCurrentUserId();
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
        var currentUserId = GetCurrentUserId();
        await _routineService.DeleteRoutineAsync(id, currentUserId);
        return NoContent();
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int.TryParse(userIdClaim, out int userId);
        return userId;
    }
}