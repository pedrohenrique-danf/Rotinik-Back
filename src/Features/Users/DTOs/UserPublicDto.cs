namespace Rotinik.Features.Users.DTOs;

public class UserPublicDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Points { get; set; }
    public int Level { get; set; }
    public int Coins { get; set; }
    public int Achievements { get; set; }
    public DateTime JoinDate { get; set; }
    public DateTime LastActivityDate { get; set; }
    public bool IsFollowed { get; set; }
}
