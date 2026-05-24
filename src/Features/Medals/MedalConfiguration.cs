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
        builder.Property(x => x.PointsThreshold).IsRequired();
    }
}