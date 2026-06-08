namespace Rotinik.Features.Medals;

public enum MedalTriggerType
{
    TotalPoints = 1,
    TasksCompleted = 2,
    RoutineStreak = 3,
    RoutinesCompleted = 4,
    PremiumPurchased = 5,
    ShopItemPurchased = 6
}

public class Medal
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    
    public MedalTriggerType TriggerType { get; set; } 
    public int TargetValue { get; set; }
    
    public int RewardPoints { get; set; } = 50;
    public int RewardCoins { get; set; } = 20;
}