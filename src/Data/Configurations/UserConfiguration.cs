using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rotinik.Models;

namespace Rotinik.Data.Configurations;

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

        builder.Property(x => x.isPremium)
            .IsRequired()
            .HasDefaultValue(false);
            
        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.UserName).IsUnique();
    }
}