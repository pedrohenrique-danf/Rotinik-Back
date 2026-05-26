namespace Rotinik.Features.Store.DTOs;

public class StoreItemCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int Price { get; set; }
    public int Category { get; set; }
}