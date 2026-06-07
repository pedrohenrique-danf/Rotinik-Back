using Rotinik.Features.Users;
using Rotinik.Features.Routines;

namespace Rotinik.Features.Tasks;

public class TaskHistory
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public TaskItem Task { get; set; } = null!;
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public TaskHistoryStatus Status { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    
    // Optional snapshot fields if needed for chart
    public int XpEarned { get; set; }
    public int CoinsEarned { get; set; }
}
