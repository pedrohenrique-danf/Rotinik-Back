namespace Rotinik.Models;

public class Routine
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public User IdUser { get; set; } = null!;
}