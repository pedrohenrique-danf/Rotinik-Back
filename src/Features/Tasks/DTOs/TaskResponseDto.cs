namespace Rotinik.Features.Tasks.DTOs;

public class TaskResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public int XpReward { get; set; }
    public int CoinReward { get; set; }
    public int Order { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int RoutineId { get; set; }
}