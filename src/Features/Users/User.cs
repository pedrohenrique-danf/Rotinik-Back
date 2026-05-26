namespace Rotinik.Features.Users;

public class User
{
    public int Id { get; set; }
    public UserRole Role { get; set; } = UserRole.User;

    public string Name { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string Password { get; set; } = string.Empty;

    public int Xp { get; set; } = 0;
    public int Coins { get; set; } = 0;
    public bool IsPremium { get; set; } = false;
    

    public ICollection<UserRefreshToken> RefreshTokens { get; set; } = new List<UserRefreshToken>();
}