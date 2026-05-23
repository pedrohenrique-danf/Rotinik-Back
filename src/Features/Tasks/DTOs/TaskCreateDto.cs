namespace Rotinik.Features.Tasks.DTOs;

public class TaskCreateDto
{
    public string Title { get; set; } = string.Empty;
    public TaskFrequency Frequency { get; set; }
    public TaskPriority Priority { get; set; }
}