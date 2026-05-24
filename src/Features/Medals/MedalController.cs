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
    public async Task<IActionResult> GetMyMedals()
    {
        var currentUserId = User.GetCurrentUserId();
        var medals = await _medalService.GetUserMedalsAsync(currentUserId);
        
        return Ok(new { data = medals });
    }
}