using Rotinik.Features.Users;

namespace Rotinik.Features.Statistics;

public enum TransactionType { Earned, Spent, Lost, Penalty }
public enum CurrencyType { Points, Coins }
public enum TransactionSource { Task, Routine, Medal, Store, System }

public class WalletTransaction
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public int Amount { get; set; }
    public CurrencyType Currency { get; set; }
    public TransactionType Type { get; set; }
    public TransactionSource Source { get; set; }
    
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}