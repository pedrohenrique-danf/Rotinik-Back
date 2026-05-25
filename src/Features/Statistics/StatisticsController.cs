using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotinik.Core.Extensions;

namespace Rotinik.Features.Statistics;

[Authorize]
[Route("api/statistics")]
[ApiController]
public class StatisticsController : ControllerBase
{
    private readonly StatisticsService _statisticsService;

    public StatisticsController(StatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    [HttpGet("dashboard")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboard()
    {
        var currentUserId = User.GetCurrentUserId();
        var stats = await _statisticsService.GetUserDashboardAsync(currentUserId);

        return Ok(new { data = stats });
    }
}