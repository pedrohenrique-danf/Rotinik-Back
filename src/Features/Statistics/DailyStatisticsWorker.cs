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

        var existingSummaries = await dbContext.Set<DailyUserSummary>()
            .Where(s => s.Date == targetDate)
            .ToDictionaryAsync(s => s.UserId, stoppingToken);

        int newRecords = 0;
        int updatedRecords = 0;

        foreach (var stat in tasksStats)
        {
            if (existingSummaries.TryGetValue(stat.UserId, out var summary))
            {
                summary.TasksCompleted = stat.TasksCompleted;
                updatedRecords++;
            }
            else
            {
                dbContext.Set<DailyUserSummary>().Add(new DailyUserSummary
                {
                    UserId = stat.UserId,
                    Date = targetDate,
                    TasksCompleted = stat.TasksCompleted,
                    RoutinesCompleted = 0,
                    PointsEarned = 0,
                    CoinsEarned = 0
                });
                newRecords++;
            }
        }

        if (newRecords > 0 || updatedRecords > 0)
        {
            await dbContext.SaveChangesAsync(stoppingToken);
            _logger.LogInformation("Successfully inserted {NewCount} and updated {UpdatedCount} user summaries for {Date}.", 
                newRecords, updatedRecords, targetDate.ToString("yyyy-MM-dd"));
        }
    }
}