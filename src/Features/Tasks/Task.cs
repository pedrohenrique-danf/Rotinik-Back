using Rotinik.Features.Routines;

namespace Rotinik.Features.Tasks;

public class TaskItem
{
    public int Id { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int RoutineId { get; set; }
    public Routine Routine { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskPriority Priority { get; set; }
    public TimeOnly ExecutionTime { get; set; }
}