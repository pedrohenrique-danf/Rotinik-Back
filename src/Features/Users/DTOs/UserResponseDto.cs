namespace Rotinik.Features.Users.DTOs;

public class UserResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Email { get; set; } = string.Empty;
    public int Points { get; set; }
    public int Coins { get; set; }
    public bool IsPremium { get; set; }
    public int RankPosition { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
}