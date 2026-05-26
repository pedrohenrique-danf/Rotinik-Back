using Rotinik.Features.Users;

namespace Rotinik.Features.Store;

public class UserItem
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int StoreItemId { get; set; }
    public StoreItem StoreItem { get; set; } = null!;

    public bool IsEquipped { get; set; } = false;
    public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
}