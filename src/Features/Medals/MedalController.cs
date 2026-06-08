using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotinik.Core.Extensions;
using Rotinik.Features.Medals.DTOs;

namespace Rotinik.Features.Medals;

[Authorize]
[Route("api/medal")]
[ApiController]
public class MedalController : ControllerBase
{
    private readonly MedalService _medalService;

    public MedalController(MedalService medalService)
    {
        _medalService = medalService;
    }

    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyMedals()
    {
        var currentUserId = User.GetCurrentUserId();
        var medals = await _medalService.GetUserMedalsAsync(currentUserId);

        return Ok(new { data = medals });
    }

    [HttpGet("debug-evaluate")]
    [AllowAnonymous]
    public async Task<IActionResult> DebugEvaluate()
    {
        var results = new List<object>();
        var users = await _medalService.Context.Users.ToListAsync();
        foreach (var u in users) 
        {
            var count = await _medalService.Context.Tasks.CountAsync(t => t.Routine.UserId == u.Id && t.IsCompleted);
            var existing = await _medalService.Context.Set<UserMedal>().Where(um => um.UserId == u.Id).Select(um => um.MedalId).ToListAsync();
            var candidates = await _medalService.Context.Medals.AsNoTracking()
                .Where(m => m.TriggerType == MedalTriggerType.TasksCompleted && !existing.Contains(m.Id))
                .ToListAsync();
            var won = candidates.Where(m => count >= m.TargetValue).ToList();
            results.Add(new { UserId = u.Id, UserName = u.UserName, Tasks = count, Existing = existing, MedalsWonNow = won.Select(w => w.Name) });
        }
        return Ok(results);
    }

    [HttpGet("all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllMedals()
    {
        var medals = await _medalService.GetAllMedalsAsync();
        return Ok(new { data = medals });
    }

    [HttpGet("progress")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetNextMedalProgress([FromQuery] int currentValue, [FromQuery] MedalTriggerType triggerType) 
    {
        if (currentValue < 0)
        {
            return BadRequest(new { message = "Current value cannot be negative." });
        }

        var progress = await _medalService.GetNextMedalProgressAsync(currentValue, triggerType);
        return Ok(new { data = progress });
    }

    [HttpPut("equip")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> EquipMedals([FromBody] MedalEquipDto dto)
    {
        var currentUserId = User.GetCurrentUserId();

        try
        {
            await _medalService.EquipMedalsAsync(currentUserId, dto);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost("admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateMedal(MedalCreateDto dto)
    {
        var result = await _medalService.CreateMedalAsync(dto);
        return StatusCode(201, result);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("admin/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMedal(int id, MedalUpdateDto dto)
    {
        await _medalService.UpdateMedalAsync(id, dto);
        return NoContent();
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("admin/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMedal(int id)
    {
        await _medalService.DeleteMedalAsync(id);
        return NoContent();
    }
}