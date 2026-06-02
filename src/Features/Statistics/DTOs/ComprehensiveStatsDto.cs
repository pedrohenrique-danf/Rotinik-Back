namespace Rotinik.Features.Statistics.DTOs;

public class ComprehensiveStatsDto
{
    public WalletStatDto Wallet { get; set; } = new();
    public PeriodSummaryDto PeriodSummary { get; set; } = new();
    public RoutineStatusDto RoutineStatus { get; set; } = new();
    public List<DailyDetailedStatDto> DailyChart { get; set; } = new();
}

public class WalletStatDto
{
    public int CurrentPoints { get; set; }
    public int CurrentCoins { get; set; }
}

public class PeriodSummaryDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalPointsEarned { get; set; }
    public int TotalCoinsEarned { get; set; }
    public int TasksCompleted { get; set; }
    public int PotentialCoinsLost { get; set; } // Moedas de tarefas atrasadas/não feitas
    public int PotentialPointsLost { get; set; } // XP de tarefas atrasadas/não feitas
}

public class RoutineStatusDto
{
    public int TotalRoutines { get; set; }
    public int RoutinesFullyCompleted { get; set; } // Todas as tarefas da rotina concluídas
    public int RoutinesPending { get; set; } // Rotinas com tarefas não concluídas
}

public class DailyDetailedStatDto
{
    public DateTime Date { get; set; }
    public int PointsEarned { get; set; }
    public int CoinsEarned { get; set; }
    public int TasksCompleted { get; set; }
}