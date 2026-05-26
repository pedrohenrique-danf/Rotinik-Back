namespace Rotinik.Features.Tasks.DTOs;

public class TaskResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
<<<<<<< HEAD
    public string? Description { get; set; }
=======
    public string Description { get; set; } = string.Empty;
    public TimeOnly ExecutionTime { get; set; }
    public string Priority { get; set; } = string.Empty;
>>>>>>> 8f6767e86801caf4e023b326a294d23f55ee89ed
    public bool IsCompleted { get; set; }
    public int XpReward { get; set; }
    public int CoinReward { get; set; }
    public int Order { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int RoutineId { get; set; }
}