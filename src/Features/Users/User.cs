namespace Rotinik.Features.Users;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsPremium { get; set; } = false;
    public int Points { get; set; } = 0;
    public int Coins { get; set; } = 0;
    public string Role { get; set; } = "user";
    public bool IsAdmin => Role == "admin";

    // O cronômetro de 30 dias para a exclusão física
    public DateTime? DeletionScheduledFor { get; set; }

    public ICollection<UserRefreshToken> RefreshTokens { get; set; } = new List<UserRefreshToken>();
}