using Microsoft.EntityFrameworkCore;
using Rotinik.Core.Data;
using Rotinik.Features.Statistics.DTOs;
using Rotinik.Features.Wallet;
namespace Rotinik.Features.Statistics;

public class StatisticsService
{
    private readonly AppDbContext _context;

    public StatisticsService(AppDbContext context)
    {
        _context = context;
    }

    private (DateTime Start, DateTime End) NormalizeDates(DateTime? start, DateTime? end)
    {
        var endDate = end?.Date ?? DateTime.UtcNow.Date;
        var startDate = start?.Date ?? endDate.AddDays(-29);
        if (startDate > endDate) (startDate, endDate) = (endDate, startDate);
        
        // Retorna Start à meia-noite e End às 23:59:59 para pegar o dia inteiro
        return (startDate, endDate.AddDays(1).AddTicks(-1));
    }

    public async Task<EconomyStatsDto> GetEconomyStatsAsync(int userId, CurrencyType currency, DateTime? start, DateTime? end)
    {
        var (startDate, endDate) = NormalizeDates(start, end);

        // Pega as transações do usuário no período
        var transactions = await _context.Set<WalletTransaction>()
            .AsNoTracking()
            .Where(t => t.UserId == userId && t.Currency == currency && t.CreatedAt >= startDate && t.CreatedAt <= endDate)
            .ToListAsync();

        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        int currentBalance = currency == CurrencyType.Points ? (user?.Points ?? 0) : (user?.Coins ?? 0);

        var chartData = new List<EconomyDailyChartDto>();
        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            var dayTx = transactions.Where(t => t.CreatedAt.Date == date).ToList();
            chartData.Add(new EconomyDailyChartDto
            {
                Date = date,
                Earned = dayTx.Where(t => t.Type == TransactionType.Earned).Sum(t => t.Amount),
                Lost = dayTx.Where(t => t.Type == TransactionType.Lost || t.Type == TransactionType.Spent).Sum(t => t.Amount)
            });
        }

        var sourceDistribution = transactions
            .Where(t => t.Type == TransactionType.Earned)
            .GroupBy(t => t.Source)
            .Select(g => new EconomySourceDistributionDto
            {
                Source = g.Key.ToString(),
                TotalAmount = g.Sum(t => t.Amount)
            }).ToList();

        return new EconomyStatsDto
        {
            CurrentBalance = currentBalance,
            TotalEarnedInPeriod = transactions.Where(t => t.Type == TransactionType.Earned).Sum(t => t.Amount),
            TotalLostOrSpentInPeriod = transactions.Where(t => t.Type == TransactionType.Lost || t.Type == TransactionType.Spent).Sum(t => t.Amount),
            DailyChart = chartData,
            EarningsBySource = sourceDistribution
        };
    }

    public async Task<TaskStatsDto> GetTaskStatsAsync(int userId, DateTime? start, DateTime? end)
    {
        var (startDate, endDate) = NormalizeDates(start, end);

        // Tarefas concluídas no período
        var completedTasks = await _context.Tasks
            .AsNoTracking()
            .Where(t => t.Routine.UserId == userId && t.IsCompleted && t.CompletedAt >= startDate && t.CompletedAt <= endDate)
            .ToListAsync();

        // Tarefas pendentes baseadas na criação da rotina (Lógica simplificada para o período)
        var pendingTasksCount = await _context.Tasks
            .AsNoTracking()
            .CountAsync(t => t.Routine.UserId == userId && !t.IsCompleted);

        var chartData = new List<DailyTaskChartDto>();
        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            chartData.Add(new DailyTaskChartDto
            {
                Date = date,
                Completed = completedTasks.Count(t => t.CompletedAt?.Date == date),
                Created = 0 // Precisaria de uma coluna CreatedAt na Tarefa para ser exato
            });
        }

        int totalCompleted = completedTasks.Count;
        int totalKnown = totalCompleted + pendingTasksCount;

        return new TaskStatsDto
        {
            TotalCreated = totalKnown,
            TotalCompleted = totalCompleted,
            TotalPending = pendingTasksCount,
            CompletionRate = totalKnown == 0 ? 0 : Math.Round((double)totalCompleted / totalKnown * 100, 2),
            DailyChart = chartData
        };
    }

    public async Task<RoutineStatsDto> GetRoutineStatsAsync(int userId, DateTime? start, DateTime? end)
    {
        var routines = await _context.Routines
            .AsNoTracking()
            .Include(r => r.Tasks)
            .Where(r => r.UserId == userId)
            .ToListAsync();

        var mostProductive = routines
            .OrderByDescending(r => r.Tasks.Where(t => t.IsCompleted).Sum(t => t.XpReward))
            .FirstOrDefault();

        return new RoutineStatsDto
        {
            TotalActiveRoutines = routines.Count,
            FullyCompletedRoutines = routines.Count(r => r.Tasks.Any() && r.Tasks.All(t => t.IsCompleted)),
            MostProductiveRoutine = mostProductive?.Title ?? "Nenhuma"
        };
    }
}