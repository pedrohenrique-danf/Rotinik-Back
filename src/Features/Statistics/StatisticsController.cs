using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotinik.Core.Extensions;
using Rotinik.Features.Statistics.DTOs;

namespace Rotinik.Features.Statistics;
using Rotinik.Features.Wallet;

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

    [HttpGet("xp")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetXpStats([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var currentUserId = User.GetCurrentUserId();
        var stats = await _statisticsService.GetEconomyStatsAsync(currentUserId, CurrencyType.Points, startDate, endDate);
        return Ok(new { data = stats });
    }

    [HttpGet("coins")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCoinStats([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var currentUserId = User.GetCurrentUserId();
        var stats = await _statisticsService.GetEconomyStatsAsync(currentUserId, CurrencyType.Coins, startDate, endDate);
        return Ok(new { data = stats });
    }

    [HttpGet("tasks")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTaskStats([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var currentUserId = User.GetCurrentUserId();
        var stats = await _statisticsService.GetTaskStatsAsync(currentUserId, startDate, endDate);
        return Ok(new { data = stats });
    }

    [HttpGet("routines")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoutineStats([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var currentUserId = User.GetCurrentUserId();
        var stats = await _statisticsService.GetRoutineStatsAsync(currentUserId, startDate, endDate);
        return Ok(new { data = stats });
    }
}