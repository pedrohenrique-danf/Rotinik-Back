using Rotinik.Features.Users;

namespace Rotinik.Features.Statistics;

public class DailyUserSummary
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public DateTime Date { get; set; }
    public int TasksCompleted { get; set; }
    public int RoutinesCompleted { get; set; }
    public int PointsEarned { get; set; }
    
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
}