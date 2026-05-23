namespace Rotinik.Features.Tasks.DTOs;

public class TaskUpdateDto
{
    public string? Title { get; set; }
    public TaskFrequency? Frequency { get; set; }
    public TaskPriority? Priority { get; set; }
}