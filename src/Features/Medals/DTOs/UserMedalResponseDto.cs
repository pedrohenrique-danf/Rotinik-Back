namespace Rotinik.Features.Medals.DTOs;

public class UserMedalResponseDto
{
    public MedalResponseDto Medal { get; set; } = new();
    public DateTime AchievedAt { get; set; }
}