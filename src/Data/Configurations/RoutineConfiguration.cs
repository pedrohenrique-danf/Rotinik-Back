using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rotinik.Models;

namespace Rotinik.Data.Configurations;

public class RoutineConfiguration : IEntityTypeConfiguration<Routine>
{
    public void Configure(EntityTypeBuilder<Routine> builder)
    {
        builder.ToTable("Routines");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Category)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne(x => x.IdUser)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);
    }
}