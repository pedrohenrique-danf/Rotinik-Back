namespace Rotinik.Features.Statistics.DTOs;

public class EconomyStatsDto
{
    public int CurrentBalance { get; set; }
    public int TotalEarnedInPeriod { get; set; }
    public int TotalLostOrSpentInPeriod { get; set; }
    public List<EconomyDailyChartDto> DailyChart { get; set; } = new();
    public List<EconomySourceDistributionDto> EarningsBySource { get; set; } = new();
}

public class EconomyDailyChartDto
{
    public DateTime Date { get; set; }
    public int Earned { get; set; }
    public int Lost { get; set; }
}

public class EconomySourceDistributionDto
{
    public string Source { get; set; } = string.Empty; // Tarefas, Medalhas, etc.
    public int TotalAmount { get; set; }
}

public class TaskStatsDto
{
    public int TotalCreated { get; set; }
    public int TotalCompleted { get; set; }
    public int TotalPending { get; set; }
    public double CompletionRate { get; set; }
    public List<DailyTaskChartDto> DailyChart { get; set; } = new();
}

public class DailyTaskChartDto
{
    public DateTime Date { get; set; }
    public int Completed { get; set; }
    public int Created { get; set; }
}

public class RoutineStatsDto
{
    public int TotalActiveRoutines { get; set; }
    public int FullyCompletedRoutines { get; set; }
    public string MostProductiveRoutine { get; set; } = string.Empty; // Nome da rotina que mais gerou XP
}