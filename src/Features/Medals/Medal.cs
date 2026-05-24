namespace Rotinik.Features.Medals;

public class Medal
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public int PointsThreshold { get; set; }
}