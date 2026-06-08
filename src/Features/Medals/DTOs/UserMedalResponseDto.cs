namespace Rotinik.Features.Medals.DTOs;

public class UserMedalResponseDto
{
    public int MedalId { get; set; }
    public MedalResponseDto Medal { get; set; } = new();
    public DateTime AchievedAt { get; set; }
    public bool IsEquipped { get; set; }
}