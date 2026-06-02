using Rotinik.Features.Routines;

namespace Rotinik.Features.Tasks;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskFrequency Frequency { get; set; }
    public TaskPriority Priority { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int XpReward { get; set; } = 10;
    public int CoinReward { get; set; } = 5;
    public int Order { get; set; } = 0;
    public int EstimatedMinutes { get; set; } = 30;
    
    public int RoutineId { get; set; }
    public Routine Routine { get; set; } = null!;
}