namespace Rotinik.Features.Tasks.DTOs;

public class TaskCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TimeOnly ExecutionTime { get; set; }
    public TaskPriority Priority { get; set; }
}