namespace Rotinik.Features.Routines.DTOs;

public class RoutineCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Frequency { get; set; } = "daily";
}