namespace Rotinik.Features.Store;

public class StoreItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int Price { get; set; }
    
    public StoreItemCategory Category { get; set; }
}