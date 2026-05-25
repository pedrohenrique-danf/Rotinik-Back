namespace Rotinik.Features.Medals.DTOs;

public class MedalProgressDto
{
    public MedalResponseDto? NextMedal { get; set; }
    public int TargetNeeded { get; set; }
}