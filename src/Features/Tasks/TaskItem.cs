using Rotinik.Features.Routines;

namespace Rotinik.Features.Tasks;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public TaskFrequency Frequency { get; set; }
    public TaskPriority Priority { get; set; }
    public bool IsCompleted { get; set; }
    public int RoutineId { get; set; }
    public Routine Routine { get; set; } = null!;
}