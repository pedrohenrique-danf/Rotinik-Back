using Rotinik.Features.Users;

namespace Rotinik.Features.Medals;

public class UserMedal
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int MedalId { get; set; }
    public Medal Medal { get; set; } = null!;

    public DateTime AchievedAt { get; set; } = DateTime.UtcNow;
}