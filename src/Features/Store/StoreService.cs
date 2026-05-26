using Microsoft.EntityFrameworkCore;
using Rotinik.Core.Exceptions;
using Rotinik.Core.Data;
using Rotinik.Features.Store.DTOs;

namespace Rotinik.Features.Store;

public class StoreService
{
    private readonly AppDbContext _context;

    public StoreService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<StoreItemResponseDto>> GetAvailableItemsAsync()
    {
        return await _context.Set<StoreItem>()
            .AsNoTracking()
            .Select(i => new StoreItemResponseDto
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
                ImageUrl = i.ImageUrl,
                Price = i.Price,
                Category = i.Category.ToString()
            })
            .ToListAsync();
    }

    public async Task<List<UserInventoryResponseDto>> GetUserInventoryAsync(int userId)
    {
        return await _context.Set<UserItem>()
            .AsNoTracking()
            .Include(ui => ui.StoreItem)
            .Where(ui => ui.UserId == userId)
            .Select(ui => new UserInventoryResponseDto
            {
                IsEquipped = ui.IsEquipped,
                PurchasedAt = ui.PurchasedAt,
                Item = new StoreItemResponseDto
                {
                    Id = ui.StoreItem.Id,
                    Name = ui.StoreItem.Name,
                    Description = ui.StoreItem.Description,
                    ImageUrl = ui.StoreItem.ImageUrl,
                    Price = ui.StoreItem.Price,
                    Category = ui.StoreItem.Category.ToString()
                }
            })
            .ToListAsync();
    }

    public async Task BuyItemAsync(int userId, int itemId)
    {
        var user = await _context.Users.FindAsync(userId) 
            ?? throw new NotFoundException("User not found.");

        var item = await _context.Set<StoreItem>().FindAsync(itemId) 
            ?? throw new NotFoundException("Store item not found.");

        var alreadyOwns = await _context.Set<UserItem>()
            .AnyAsync(ui => ui.UserId == userId && ui.StoreItemId == itemId);

        if (alreadyOwns)
            throw new ConflictException("You already own this item.");

        if (user.Coins < item.Price)
            throw new ConflictException("Not enough coins to purchase this item.");

        user.Coins -= item.Price;

        var userItem = new UserItem
        {
            UserId = userId,
            StoreItemId = itemId,
            IsEquipped = false,
            PurchasedAt = DateTime.UtcNow
        };

        await _context.Set<UserItem>().AddAsync(userItem);
        await _context.SaveChangesAsync();
    }

    public async Task EquipItemAsync(int userId, int itemId)
    {
        var inventory = await _context.Set<UserItem>()
            .Include(ui => ui.StoreItem)
            .Where(ui => ui.UserId == userId)
            .ToListAsync();

        var itemToEquip = inventory.SingleOrDefault(ui => ui.StoreItemId == itemId) 
            ?? throw new NotFoundException("Item not found in your inventory.");

        if (itemToEquip.IsEquipped)
            return;

        var currentlyEquipped = inventory.Where(ui => 
            ui.IsEquipped && 
            ui.StoreItem.Category == itemToEquip.StoreItem.Category);

        foreach (var item in currentlyEquipped)
        {
            item.IsEquipped = false;
        }

        itemToEquip.IsEquipped = true;

        await _context.SaveChangesAsync();
    }

    public async Task<StoreItemResponseDto> CreateStoreItemAsync(StoreItemCreateDto dto)
{
    var item = new StoreItem
    {
        Name = dto.Name,
        Description = dto.Description,
        ImageUrl = dto.ImageUrl,
        Price = dto.Price,
        Category = (StoreItemCategory)dto.Category
    };

    await _context.Set<StoreItem>().AddAsync(item);
    await _context.SaveChangesAsync();

    return new StoreItemResponseDto
    {
        Id = item.Id,
        Name = item.Name,
        Description = item.Description,
        ImageUrl = item.ImageUrl,
        Price = item.Price,
        Category = item.Category.ToString()
    };
    }
}