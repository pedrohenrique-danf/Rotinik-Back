using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rotinik.Features.Medals;

namespace Rotinik.Features.Users;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.UserName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Password)
            .IsRequired();

        builder.Property(x => x.BirthDate)
            .IsRequired();

        builder.Property(x => x.IsPremium)
            .IsRequired()
            .HasDefaultValue(false);
        
        builder.Property(x => x.Points)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.Coins)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.Role)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("user");

        builder.Property(x => x.JoinDate)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.LastActivityDate)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Mapeamento explícito do relacionamento das Medalhas
        builder.HasMany<Medal>()
        .WithMany()
        .UsingEntity<UserMedal>(
            j => j.HasOne(um => um.Medal).WithMany().HasForeignKey(um => um.MedalId),
            j => j.HasOne(um => um.User).WithMany().HasForeignKey(um => um.UserId),
            j => {
                j.HasKey(um => new { um.UserId, um.MedalId });
                j.ToTable("UserMedals");
            });

        // MAPEAMENTO CORRIGIDO: Explicitando a relação e a tabela de RefreshTokens
        builder.HasMany(x => x.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new User
            {
                Id = 1,
                Name = "Administrador",
                UserName = "admin",
                Email = "admin@rotinik.com",
                Password = "$2a$11$ah3WykwyffrfPRSXBSynZOnXqdFPrVjJeHB6vTmN47FIn1rPJ.9wq",
                BirthDate = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsPremium = true,
                Role = "admin",
                Points = 1000,
                Coins = 1000,
                JoinDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                LastActivityDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.UserName).IsUnique();
    }
}