using Rotinik.Features.Users;
using Rotinik.Features.Tasks;

namespace Rotinik.Features.Routines;

public class Routine
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Frequency { get; set; } = "daily";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public bool IsDefault { get; set; } = false;
}