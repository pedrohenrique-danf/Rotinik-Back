using Rotinik.Features.Users;

namespace Rotinik.Features.Statistics;

public class DailyUserSummary
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public DateTime Date { get; set; }
    
    public int PointsEarned { get; set; } = 0;
    public int CoinsEarned { get; set; } = 0;
    
    public int TasksCompleted { get; set; } = 0;
    public int RoutinesCompleted { get; set; } = 0;
}