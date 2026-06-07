namespace Rotinik.Features.Shop.DTOs;

public class ShopItemUpdateDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Category { get; set; }
    public int? Price { get; set; }
    public string? Rarity { get; set; }
    public double? Discount { get; set; }
    public bool? IsNew { get; set; }
}
