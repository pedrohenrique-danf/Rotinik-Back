namespace Rotinik.Features.Users.DTOs;

public class AdminUserUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Role { get; set; } = "user";
    public int Points { get; set; }
    public int Coins { get; set; }
    public bool IsPremium { get; set; }
}
