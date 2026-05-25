namespace Rotinik.Features.Users;

public class UserRefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiryTime { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}