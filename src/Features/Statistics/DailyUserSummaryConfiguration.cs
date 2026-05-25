using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Rotinik.Features.Statistics;

public class DailyUserSummaryConfiguration : IEntityTypeConfiguration<DailyUserSummary>
{
    public void Configure(EntityTypeBuilder<DailyUserSummary> builder)
    {
        builder.ToTable("DailyUserSummaries");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.UserId, x.Date }).IsUnique();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}