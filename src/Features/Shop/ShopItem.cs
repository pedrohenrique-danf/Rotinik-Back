namespace Rotinik.Features.Shop;

public class ShopItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // cosmetic, boost, theme, badge
    public int Price { get; set; }
    public string Rarity { get; set; } = string.Empty; // common, rare, epic, legendary
    public double? Discount { get; set; } // Discount percentage if applicable
    public bool IsNew { get; set; }
}
