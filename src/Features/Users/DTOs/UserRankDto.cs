namespace Rotinik.Features.Users.DTOs;

public class UserRankDto
{
    public int RankPosition { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int Points { get; set; }
    public bool isPremium { get; set; }
}