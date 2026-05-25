namespace Rotinik.Features.Tasks.DTOs;

public class TaskResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TimeOnly ExecutionTime { get; set; }
    public string Priority { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int RoutineId { get; set; }
}