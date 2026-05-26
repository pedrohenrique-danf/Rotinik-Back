namespace Rotinik.Features.Routines.DTOs;

public class RoutineUpdateDto
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
}