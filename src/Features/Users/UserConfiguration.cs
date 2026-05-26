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

        builder.Property(x => x.Role)
            .IsRequired()
            .HasDefaultValue(UserRole.User);

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
        
        builder.Property(x => x.Xp)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.Coins)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasMany<Medal>()
        .WithMany()
        .UsingEntity<UserMedal>(
            j => j.HasOne(um => um.Medal).WithMany().HasForeignKey(um => um.MedalId),
            j => j.HasOne(um => um.User).WithMany().HasForeignKey(um => um.UserId),
            j => {
                j.HasKey(um => new { um.UserId, um.MedalId });
                j.ToTable("UserMedals");
            });

        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.UserName).IsUnique();
    }
}