namespace Rotinik.Features.Store.DTOs;

public class UserInventoryResponseDto
{
    public StoreItemResponseDto Item { get; set; } = new();
    public bool IsEquipped { get; set; }
    public DateTime PurchasedAt { get; set; }
}