using Rotinik.Features.Routines;

namespace Rotinik.Features.Tasks;

public class TaskItem
{
    public int Id { get; set; }
<<<<<<< HEAD
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskFrequency Frequency { get; set; }
    public TaskPriority Priority { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int XpReward { get; set; } = 10;
    public int CoinReward { get; set; } = 5;
    public int Order { get; set; } = 0;
    
=======
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
>>>>>>> 8f6767e86801caf4e023b326a294d23f55ee89ed
    public int RoutineId { get; set; }
    public Routine Routine { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskPriority Priority { get; set; }
    public TimeOnly ExecutionTime { get; set; }
}