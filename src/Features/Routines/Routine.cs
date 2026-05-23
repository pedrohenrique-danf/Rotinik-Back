using Rotinik.Features.Users;
using Rotinik.Features.Tasks;

namespace Rotinik.Features.Routines;

public class Routine
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public User IdUser { get; set; } = null!;
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public bool IsDefault { get; set; } = false;
}