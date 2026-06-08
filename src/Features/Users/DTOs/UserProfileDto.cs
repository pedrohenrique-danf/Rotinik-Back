namespace Rotinik.Features.Users.DTOs;

public class UserProfileDto
{
    public string Name { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public int Points { get; set; }
    public bool IsPremium { get; set; }
    public int RankPosition { get; set; }
    
    // Key: Category, Value: Icon/CSS value
    public Dictionary<string, string> EquippedCosmetics { get; set; } = new();
}