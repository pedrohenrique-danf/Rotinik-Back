namespace Rotinik.Features.Medals.DTOs;

public class MedalCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public MedalTriggerType TriggerType { get; set; }
    public int TargetValue { get; set; }
}
