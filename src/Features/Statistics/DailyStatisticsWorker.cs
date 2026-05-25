using Microsoft.EntityFrameworkCore;
using Rotinik.Core.Data;

namespace Rotinik.Features.Statistics.Workers;

public class DailyStatisticsWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DailyStatisticsWorker> _logger;

    public DailyStatisticsWorker(IServiceProvider serviceProvider, ILogger<DailyStatisticsWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Daily Statistics Worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            var nextMidnight = now.Date.AddDays(1);
            var timeUntilMidnight = nextMidnight - now;

            _logger.LogInformation("Worker sleeping for {Hours}h {Minutes}m until midnight UTC.", 
                timeUntilMidnight.Hours, timeUntilMidnight.Minutes);

            await Task.Delay(timeUntilMidnight, stoppingToken);

            try
            {
                await ProcessYesterdayStatisticsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing daily statistics.");
            }
        }
    }

    private async Task ProcessYesterdayStatisticsAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Processing daily summaries...");

        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var targetDate = DateTime.UtcNow.Date.AddDays(-1);

        var tasksStats = await dbContext.Tasks
            .Where(t => t.CompletedAt >= targetDate && t.CompletedAt < targetDate.AddDays(1) && t.IsCompleted)
            .GroupBy(t => t.Routine.UserId)
            .Select(g => new { UserId = g.Key, TasksCompleted = g.Count() })
            .ToListAsync(stoppingToken);

        var summariesToInsert = new List<DailyUserSummary>();

        foreach (var stat in tasksStats)
        {
            summariesToInsert.Add(new DailyUserSummary
            {
                UserId = stat.UserId,
                Date = targetDate,
                TasksCompleted = stat.TasksCompleted,
                RoutinesCompleted = 0,
                PointsEarned = 0
            });
        }

        if (summariesToInsert.Any())
        {
            await dbContext.Set<DailyUserSummary>().AddRangeAsync(summariesToInsert);
            await dbContext.SaveChangesAsync(stoppingToken);
            _logger.LogInformation("Successfully saved {Count} user summaries for {Date}.", summariesToInsert.Count, targetDate.ToString("yyyy-MM-dd"));
        }
    }
}