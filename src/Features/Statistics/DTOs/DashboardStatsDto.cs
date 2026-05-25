namespace Rotinik.Features.Statistics.DTOs;

public class DashboardStatsDto
{
    public int TotalTasksCompleted { get; set; }
    public List<DailyStatDto> Last7Days { get; set; } = new();
}

public class DailyStatDto
{
    public DateTime Date { get; set; }
    public int TasksCompleted { get; set; }
}