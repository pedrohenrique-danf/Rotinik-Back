using Microsoft.EntityFrameworkCore;
using Rotinik.Core.Data;
using Rotinik.Features.Statistics.DTOs;

namespace Rotinik.Features.Statistics;

public class StatisticsService
{
    private readonly AppDbContext _context;

    public StatisticsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStatsDto> GetUserDashboardAsync(int userId)
    {
        var today = DateTime.UtcNow.Date;
        var sixDaysAgo = today.AddDays(-6);

        var historicalStats = await _context.Set<DailyUserSummary>()
            .AsNoTracking()
            .Where(s => s.UserId == userId && s.Date >= sixDaysAgo && s.Date < today)
            .Select(s => new DailyStatDto
            {
                Date = s.Date,
                TasksCompleted = s.TasksCompleted
            })
            .ToListAsync();

        var tasksCompletedToday = await _context.Tasks
            .AsNoTracking()
            .CountAsync(t => t.Routine.UserId == userId && t.IsCompleted && t.CompletedAt >= today);

        var todayStat = new DailyStatDto
        {
            Date = today,
            TasksCompleted = tasksCompletedToday
        };

        var completeChartData = new List<DailyStatDto>();
        
        for (int i = 0; i <= 6; i++)
        {
            var targetDate = sixDaysAgo.AddDays(i);
            
            if (targetDate == today)
            {
                completeChartData.Add(todayStat);
            }
            else
            {
                var history = historicalStats.FirstOrDefault(h => h.Date == targetDate);
                completeChartData.Add(history ?? new DailyStatDto { Date = targetDate, TasksCompleted = 0 });
            }
        }

        var totalHistoricalTasks = await _context.Set<DailyUserSummary>()
            .Where(s => s.UserId == userId)
            .SumAsync(s => s.TasksCompleted);

        return new DashboardStatsDto
        {
            TotalTasksCompleted = totalHistoricalTasks + tasksCompletedToday,
            Last7Days = completeChartData.OrderBy(x => x.Date).ToList()
        };
    }
}