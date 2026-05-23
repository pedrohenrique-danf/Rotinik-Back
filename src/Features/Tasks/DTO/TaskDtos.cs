namespace Rotinik.Features.Tasks.DTO;

public class TaskCreateDto
{
    public string Title { get; set; } = string.Empty;
    public TaskFrequency Frequency { get; set; }
    public TaskPriority Priority { get; set; }
}

public class TaskUpdateDto
{
    public string? Title { get; set; }
    public TaskFrequency? Frequency { get; set; }
    public TaskPriority? Priority { get; set; }
}

public class TaskResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public int RoutineId { get; set; }
}