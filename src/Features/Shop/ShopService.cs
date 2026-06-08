using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rotinik.Core.Data;
using Rotinik.Core.Exceptions;
using Rotinik.Features.Shop.DTOs;

namespace Rotinik.Features.Shop;

public class ShopService
{
    private readonly AppDbContext _context;

    public ShopService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ShopItemResponseDto>> ListAllItemsAsync(int currentUserId)
    {
        var user = await _context.Users.FindAsync(currentUserId);
        bool isPremium = user?.IsPremium ?? false;

        var items = await _context.ShopItems
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync();

        return items.Select(x => {
            var dto = new ShopItemResponseDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Icon = x.Icon,
                Category = x.Category,
                Price = x.Price,
                Rarity = x.Rarity,
                Discount = x.Discount,
                IsNew = x.IsNew
            };

            if (isPremium)
            {
                double currentDiscount = dto.Discount ?? 0.0;
                dto.Discount = System.Math.Max(currentDiscount, 15.0);
            }

            return dto;
        }).ToList();
    }

    public async Task<ShopItemResponseDto> CreateItemAsync(ShopItemCreateDto dto)
    {
        var item = new ShopItem
        {
            Name = dto.Name,
            Description = dto.Description,
            Icon = dto.Icon,
            Category = dto.Category,
            Price = dto.Price,
            Rarity = dto.Rarity,
            Discount = dto.Discount,
            IsNew = dto.IsNew
        };

        await _context.ShopItems.AddAsync(item);
        await _context.SaveChangesAsync();

        return new ShopItemResponseDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Icon = item.Icon,
            Category = item.Category,
            Price = item.Price,
            Rarity = item.Rarity,
            Discount = item.Discount,
            IsNew = item.IsNew
        };
    }

    public async Task UpdateItemAsync(int id, ShopItemUpdateDto dto)
    {
        var item = await _context.ShopItems.FindAsync(id);
        if (item == null)
            throw new NotFoundException("Shop item not found.");

        if (dto.Name != null) item.Name = dto.Name;
        if (dto.Description != null) item.Description = dto.Description;
        if (dto.Icon != null) item.Icon = dto.Icon;
        if (dto.Category != null) item.Category = dto.Category;
        if (dto.Price.HasValue) item.Price = dto.Price.Value;
        if (dto.Rarity != null) item.Rarity = dto.Rarity;
        if (dto.Discount != null) item.Discount = dto.Discount; // wait, in C#, we use nullable, so if it's passed we update it.
        if (dto.IsNew.HasValue) item.IsNew = dto.IsNew.Value;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteItemAsync(int id)
    {
        var item = await _context.ShopItems.FindAsync(id);
        if (item == null)
            throw new NotFoundException("Shop item not found.");

        _context.ShopItems.Remove(item);
        await _context.SaveChangesAsync();
    }
}
