using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotinik.Core.Extensions;

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
    public async Task<IActionResult> GetNextMedalProgress([FromQuery] int currentUserPoints) 
    {
        if (currentUserPoints < 0)
        {
            return BadRequest(new { message = "Current user points cannot be negative." });
        }

        var progress = await _medalService.GetNextMedalProgressAsync(currentUserPoints);
        return Ok(new { data = progress });
    }
}