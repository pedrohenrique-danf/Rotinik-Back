using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rotinik.Core.Data;
using Rotinik.Core.Exceptions;
using Rotinik.Features.Shop.DTOs;
using Rotinik.Features.Wallet; // Necessário para os Enums da Carteira

namespace Rotinik.Features.Shop;

public class ShopService
{
    private readonly AppDbContext _context;

    public ShopService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ShopItemResponseDto>> ListAllItemsAsync()
    {
        var items = await _context.ShopItems
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new ShopItemResponseDto
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
            })
            .ToListAsync();

        return items;
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
        if (dto.Discount != null) item.Discount = dto.Discount;
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

    // O CÉREBRO DA COMPRA (REQUISITO FMG_5)
    public async Task PurchaseItemAsync(int shopItemId, int userId)
    {
        // 1. Validar a existência do Item e do Usuário
        var item = await _context.ShopItems.FindAsync(shopItemId);
        if (item == null)
            throw new NotFoundException("Item da loja não encontrado.");

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new NotFoundException("Usuário não encontrado.");

        // 2. REGRA FMG_5: Blindagem de saldo
        if (user.Coins < item.Price)
            throw new InvalidOperationException("Saldo insuficiente para esta compra.");

        // --- INÍCIO DO BLOCO ATÔMICO ---

        // 3. Deduzir o saldo do usuário
        user.Coins -= item.Price;

        // 4. Registrar no Inventário (A entrega do item)
        var userInventoryItem = new UserShopItem
        {
            UserId = userId,
            ShopItemId = shopItemId,
            PurchasedAt = DateTime.UtcNow
        };
        await _context.UserShopItems.AddAsync(userInventoryItem);

        // 5. Auditar a transação na Carteira (Com os Enums corretos)
        var transaction = new WalletTransaction
        {
            UserId = userId,
            Amount = -item.Price,
            Currency = CurrencyType.Coins,
            Type = TransactionType.Spent,
            Source = TransactionSource.Store,
            Description = $"Compra na loja: {item.Name}",
            CreatedAt = DateTime.UtcNow
        };
        await _context.WalletTransactions.AddAsync(transaction);

        // --- FIM DO BLOCO ATÔMICO ---

        await _context.SaveChangesAsync();
    }
}