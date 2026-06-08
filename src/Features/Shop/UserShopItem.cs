using Rotinik.Features.Users;

namespace Rotinik.Features.Shop;

public class UserShopItem
{
    public int Id { get; set; }
    public int UserId { get; set; } 
    public int ShopItemId { get; set; }
    public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
    public bool IsEquipped { get; set; } = false;

    public User User { get; set; } = null!;
    public ShopItem ShopItem { get; set; } = null!;
}