namespace Rotinik.Features.Users.DTOs;

public class UserRankDto
{
    public string UserName { get; set; } = string.Empty;
    public int RankPosition { get; set; }
    public int Xp { get; set; } 
    public bool IsPremium { get; set; }
}