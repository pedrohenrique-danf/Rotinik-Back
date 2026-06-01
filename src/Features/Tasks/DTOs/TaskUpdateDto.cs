namespace Rotinik.Features.Tasks.DTOs;

public class TaskUpdateDto
{
    public string? Title { get; set; }
    public TaskFrequency? Frequency { get; set; }
    public string? Importance { get; set; }
    public int? EstimatedMinutes { get; set; }
}