using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Rotinik.Features.Shop;

public class ShopItemConfiguration : IEntityTypeConfiguration<ShopItem>
{
    public void Configure(EntityTypeBuilder<ShopItem> builder)
    {
        builder.ToTable("ShopItems");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(500);
        builder.Property(x => x.Icon).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Category).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Price).IsRequired();
        builder.Property(x => x.Rarity).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Discount).IsRequired(false);
        builder.Property(x => x.IsNew).IsRequired().HasDefaultValue(false);

        // Seeding initial shop catalog
        builder.HasData(
            new ShopItem
            {
                Id = 1,
                Name = "Gato Ninja",
                Description = "Um gato ninja como mascote",
                Icon = "🐱",
                Category = "avatar",
                Price = 150,
                Rarity = "rare",
                IsNew = true
            },
            new ShopItem
            {
                Id = 2,
                Name = "Dragao Roxo",
                Description = "Um dragao mistico roxo",
                Icon = "🐉",
                Category = "avatar",
                Price = 250,
                Rarity = "epic",
                IsNew = false
            },
            new ShopItem
            {
                Id = 3,
                Name = "Unicornio Brilhoso",
                Description = "Um unicornio com brilho especial",
                Icon = "🦄",
                Category = "avatar",
                Price = 200,
                Rarity = "epic",
                IsNew = true
            },
            new ShopItem
            {
                Id = 4,
                Name = "Dobro de XP (7 dias)",
                Description = "Ganhe o dobro de XP pelas proximas 7 dias",
                Icon = "⚡",
                Category = "navbar",
                Price = 500,
                Rarity = "rare",
                IsNew = false
            },
            new ShopItem
            {
                Id = 5,
                Name = "Protetor de Streak",
                Description = "Proteja seu streak por 1 falha",
                Icon = "🛡️",
                Category = "border",
                Price = 300,
                Rarity = "epic",
                IsNew = false
            },
            new ShopItem
            {
                Id = 6,
                Name = "Tema Neon",
                Description = "Tema com cores neon brilhantes",
                Icon = "💎",
                Category = "background",
                Price = 200,
                Rarity = "rare",
                IsNew = true
            },
            new ShopItem
            {
                Id = 7,
                Name = "Tema Floresta",
                Description = "Tema com cores verdes naturais",
                Icon = "🌿",
                Category = "background",
                Price = 150,
                Rarity = "common",
                IsNew = false
            },
            new ShopItem
            {
                Id = 8,
                Name = "Placa: Speedrunner",
                Description = "Mostra que voce e rapido",
                Icon = "🏃",
                Category = "level_icon",
                Price = 100,
                Rarity = "common",
                IsNew = false
            },
            new ShopItem
            {
                Id = 9,
                Name = "Placa: Lenda",
                Description = "A placa do verdadeiro lendario",
                Icon = "👑",
                Category = "level_icon",
                Price = 1000,
                Rarity = "legendary",
                IsNew = false
            }
        );
    }
}
