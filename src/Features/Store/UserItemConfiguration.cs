using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Rotinik.Features.Store;

public class UserItemConfiguration : IEntityTypeConfiguration<UserItem>
{
    public void Configure(EntityTypeBuilder<UserItem> builder)
    {
        builder.ToTable("UserItems");
        
        // Chave composta para garantir que o usuário só tenha uma cópia do item
        builder.HasKey(x => new { x.UserId, x.StoreItemId });

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.StoreItem)
            .WithMany()
            .HasForeignKey(x => x.StoreItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.IsEquipped).IsRequired().HasDefaultValue(false);
        builder.Property(x => x.PurchasedAt).IsRequired();
    }
}