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
        builder.Property(x => x.Icon).IsRequired().HasMaxLength(255);
        builder.Property(x => x.Category).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Price).IsRequired();
        builder.Property(x => x.Rarity).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Discount).IsRequired(false);
        builder.Property(x => x.IsNew).IsRequired().HasDefaultValue(false);

        // Seeding initial shop catalog
        builder.HasData(
            // --- AVATARES (5) ---
            new ShopItem { Id = 1, Name = "Astronauta Novato", Description = "O inicio de uma grande jornada espacial.", Icon = "👨‍🚀", Category = "avatar", Price = 100, Rarity = "common", IsNew = true },
            new ShopItem { Id = 2, Name = "Extraterrestre", Description = "Um visitante de outra galaxia.", Icon = "👽", Category = "avatar", Price = 200, Rarity = "rare", IsNew = false },
            new ShopItem { Id = 3, Name = "Robo Marciano", Description = "Tecnologia avancada de marte.", Icon = "🤖", Category = "avatar", Price = 300, Rarity = "rare", IsNew = true },
            new ShopItem { Id = 4, Name = "Comandante Estelar", Description = "Lider da frota espacial.", Icon = "🧑‍✈️", Category = "avatar", Price = 500, Rarity = "epic", IsNew = false },
            new ShopItem { Id = 5, Name = "Entidade Cosmica", Description = "O puro poder do universo.", Icon = "🌌", Category = "avatar", Price = 1000, Rarity = "legendary", IsNew = false },

            // --- BORDAS DE PERFIL (5) ---
            new ShopItem { Id = 6, Name = "Borda de Asteroides", Description = "Poeira espacial rodando seu perfil.", Icon = "2px dashed #9ca3af", Category = "border", Price = 150, Rarity = "common", IsNew = false },
            new ShopItem { Id = 7, Name = "Borda Neon Orbita", Description = "Um aro de luz de orbita baixa.", Icon = "3px solid #38bdf8", Category = "border", Price = 250, Rarity = "rare", IsNew = true },
            new ShopItem { Id = 8, Name = "Borda Chama de Foguete", Description = "A propulsao do foguete no seu perfil.", Icon = "3px solid #f97316", Category = "border", Price = 350, Rarity = "epic", IsNew = false },
            new ShopItem { Id = 9, Name = "Borda Materia Escura", Description = "Uma aura cosmica misteriosa.", Icon = "4px double #8b5cf6", Category = "border", Price = 600, Rarity = "epic", IsNew = true },
            new ShopItem { Id = 10, Name = "Borda Supernova", Description = "O brilho intenso de uma estrela explodindo.", Icon = "4px solid #fbbf24", Category = "border", Price = 1200, Rarity = "legendary", IsNew = false },

            // --- ICONES DE NIVEL (5) ---
            new ShopItem { Id = 11, Name = "Satelite", Description = "Orbita basica de comunicacao.", Icon = "📡", Category = "level_icon", Price = 100, Rarity = "common", IsNew = false },
            new ShopItem { Id = 12, Name = "Lua", Description = "Exploracao do nosso vizinho.", Icon = "🌕", Category = "level_icon", Price = 200, Rarity = "common", IsNew = false },
            new ShopItem { Id = 13, Name = "Foguete Espacial", Description = "Voando alem das nuvens.", Icon = "🚀", Category = "level_icon", Price = 400, Rarity = "rare", IsNew = true },
            new ShopItem { Id = 14, Name = "Disco Voador", Description = "Uma nave nao identificada.", Icon = "🛸", Category = "level_icon", Price = 700, Rarity = "epic", IsNew = false },
            new ShopItem { Id = 15, Name = "Estrela Cadente", Description = "Um rastro iluminado e magico.", Icon = "🌠", Category = "level_icon", Price = 1500, Rarity = "legendary", IsNew = true },

            // --- FUNDO DOS CARDS E MODAIS (5) ---
            new ShopItem { Id = 16, Name = "Fundo Superficie Lunar", Description = "Cores cinzas do solo lunar.", Icon = "linear-gradient(135deg, #1f2937, #374151)", Category = "background", Price = 200, Rarity = "common", IsNew = false },
            new ShopItem { Id = 17, Name = "Fundo Ceu Estrelado", Description = "Uma vista para as estrelas noturnas.", Icon = "linear-gradient(135deg, #0f172a, #1e1b4b)", Category = "background", Price = 400, Rarity = "rare", IsNew = false },
            new ShopItem { Id = 18, Name = "Fundo Nebulosa Solar", Description = "Uma mistura quente de gases estelares.", Icon = "linear-gradient(135deg, #7c2d12, #9a3412)", Category = "background", Price = 600, Rarity = "epic", IsNew = true },
            new ShopItem { Id = 19, Name = "Fundo Buraco Negro", Description = "Sugando toda a luz do universo.", Icon = "radial-gradient(circle, #000000 0%, #171717 100%)", Category = "background", Price = 900, Rarity = "epic", IsNew = false },
            new ShopItem { Id = 20, Name = "Fundo Aurora Boreal Estelar", Description = "Cores cosmicas brilhantes.", Icon = "linear-gradient(135deg, #1e1b4b 0%, #4c1d95 50%, #0ea5e9 100%)", Category = "background", Price = 2000, Rarity = "legendary", IsNew = true },

            // --- NAV BARS (5) ---
            new ShopItem { Id = 21, Name = "Nav Bar Padrao Espacial", Description = "Metal basico de naves.", Icon = "#1e293b", Category = "navbar", Price = 150, Rarity = "common", IsNew = false },
            new ShopItem { Id = 22, Name = "Nav Bar Ametista Galactica", Description = "Brilho roxo espacial profundo.", Icon = "#4c1d95", Category = "navbar", Price = 300, Rarity = "rare", IsNew = true },
            new ShopItem { Id = 23, Name = "Nav Bar Marte", Description = "Vermelho e quente como o planeta.", Icon = "#9f1239", Category = "navbar", Price = 500, Rarity = "epic", IsNew = false },
            new ShopItem { Id = 24, Name = "Nav Bar Via Lactea", Description = "Um tom cosmico azul escuro.", Icon = "#0f172a", Category = "navbar", Price = 800, Rarity = "epic", IsNew = false },
            new ShopItem { Id = 25, Name = "Nav Bar Velocidade da Luz", Description = "O brilho maximo da tecnologia.", Icon = "linear-gradient(90deg, #6d28d9, #2563eb, #db2777)", Category = "navbar", Price = 2500, Rarity = "legendary", IsNew = true }
        );
    }
}
