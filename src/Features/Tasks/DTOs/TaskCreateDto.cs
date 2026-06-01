namespace Rotinik.Features.Tasks.DTOs;

public class TaskCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int XpReward { get; set; } = 10;
    public int CoinReward { get; set; } = 5;
    public TaskFrequency Frequency { get; set; } = TaskFrequency.None;
    public string Importance { get; set; } = "media";
    public int EstimatedMinutes { get; set; } = 30;
}