using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Rotinik.Features.Medals;

public class MedalConfiguration : IEntityTypeConfiguration<Medal>
{
    public void Configure(EntityTypeBuilder<Medal> builder)
    {
        builder.ToTable("Medals");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(255);
        builder.Property(x => x.IconUrl).IsRequired().HasMaxLength(500);
        
        builder.Property(x => x.TriggerType).IsRequired();
        builder.Property(x => x.TargetValue).IsRequired();

        // 10 Medalhas Base
        builder.HasData(
            new Medal { Id = 1, Name = "Primeiro Passo", Description = "Conclua sua 1ª tarefa.", IconUrl = "🎯", TriggerType = MedalTriggerType.TasksCompleted, TargetValue = 1, RewardPoints = 50, RewardCoins = 10 },
            new Medal { Id = 2, Name = "Produtivo", Description = "Conclua 10 tarefas.", IconUrl = "⚡", TriggerType = MedalTriggerType.TasksCompleted, TargetValue = 10, RewardPoints = 100, RewardCoins = 50 },
            new Medal { Id = 3, Name = "Máquina de Tarefas", Description = "Conclua 50 tarefas.", IconUrl = "🔥", TriggerType = MedalTriggerType.TasksCompleted, TargetValue = 50, RewardPoints = 500, RewardCoins = 200 },
            new Medal { Id = 4, Name = "Rotineiro", Description = "Conclua sua primeira rotina.", IconUrl = "📅", TriggerType = MedalTriggerType.RoutinesCompleted, TargetValue = 1, RewardPoints = 100, RewardCoins = 25 },
            new Medal { Id = 5, Name = "Firme e Forte", Description = "Conclua 3 rotinas.", IconUrl = "💪", TriggerType = MedalTriggerType.RoutinesCompleted, TargetValue = 3, RewardPoints = 300, RewardCoins = 100 },
            new Medal { Id = 6, Name = "Mestre da Rotina", Description = "Conclua 10 rotinas.", IconUrl = "👑", TriggerType = MedalTriggerType.RoutinesCompleted, TargetValue = 10, RewardPoints = 1000, RewardCoins = 500 },
            new Medal { Id = 7, Name = "Acumulador", Description = "Junte 100 Moedas.", IconUrl = "💰", TriggerType = MedalTriggerType.TotalPoints, TargetValue = 100, RewardPoints = 150, RewardCoins = 0 },
            new Medal { Id = 8, Name = "Rico", Description = "Junte 500 Moedas.", IconUrl = "💎", TriggerType = MedalTriggerType.TotalPoints, TargetValue = 500, RewardPoints = 500, RewardCoins = 0 },
            new Medal { Id = 9, Name = "Apoiador Premium", Description = "Adquira a versão Premium.", IconUrl = "⭐", TriggerType = MedalTriggerType.PremiumPurchased, TargetValue = 1, RewardPoints = 2000, RewardCoins = 1000 },
            new Medal { Id = 10, Name = "Consumidor", Description = "Compre seu primeiro cosmético.", IconUrl = "🛍️", TriggerType = MedalTriggerType.ShopItemPurchased, TargetValue = 1, RewardPoints = 50, RewardCoins = 10 }
        );
    }
}