namespace Rotinik.Features.Tasks.DTOs;

public class TaskCreateDto
{
    public string Title { get; set; } = string.Empty;
<<<<<<< HEAD
    public string? Description { get; set; }
    public int XpReward { get; set; } = 10;
    public int CoinReward { get; set; } = 5;
    public TaskFrequency Frequency { get; set; } = TaskFrequency.None;
    public TaskPriority Priority { get; set; } = TaskPriority.Moderate;
=======
    public string Description { get; set; } = string.Empty;
    public TimeOnly ExecutionTime { get; set; }
    public TaskPriority Priority { get; set; }
>>>>>>> 8f6767e86801caf4e023b326a294d23f55ee89ed
}