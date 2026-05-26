namespace Rotinik.Features.Routines.DTOs;

public class RoutineResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}