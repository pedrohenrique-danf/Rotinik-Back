namespace Rotinik.Features.Shop.DTOs;

public class ShopItemResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Price { get; set; }
    public string Rarity { get; set; } = string.Empty;
    public double? Discount { get; set; }
    public bool IsNew { get; set; }
}
