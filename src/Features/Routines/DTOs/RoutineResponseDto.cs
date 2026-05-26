using Rotinik.Features.Tasks.DTOs;

namespace Rotinik.Features.Routines.DTOs;

public class RoutineResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Frequency { get; set; } = "daily";
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<TaskResponseDto> Tasks { get; set; } = new();
    public int TotalXp => Tasks.Sum(t => t.XpReward);
    public int TotalCoins => Tasks.Sum(t => t.CoinReward);
    public bool IsCompleted => Tasks.Count > 0 && Tasks.All(t => t.IsCompleted);
    public int CompletionStreak { get; set; } = 0;
    public DateTime? LastCompletedAt { get; set; }
}