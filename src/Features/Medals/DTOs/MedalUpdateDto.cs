namespace Rotinik.Features.Medals.DTOs;

public class MedalUpdateDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public MedalTriggerType? TriggerType { get; set; }
    public int? TargetValue { get; set; }
    public int? RewardPoints { get; set; }
    public int? RewardCoins { get; set; }
}
