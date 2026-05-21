namespace Rotinik.Models;

public class Payment
{
    public int Id { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }
    public User User { get; set; } = null!;
}