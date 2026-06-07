namespace Rotinik.Features.Tasks.DTOs;

public class TaskUpdateDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public TaskFrequency? Frequency { get; set; }
    public string? Importance { get; set; }
    public string? DeadlineValue { get; set; }
}