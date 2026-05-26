using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Rotinik.Features.Tasks;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("Tasks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(TaskConstants.TitleMaxLength); 

        builder.Property(x => x.Description)
            .HasMaxLength(TaskConstants.DescriptionMaxLength);

        builder.Property(x => x.ExecutionTime)
            .IsRequired();

        builder.Property(x => x.Priority)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.IsCompleted)
            .HasDefaultValue(false);

        builder.Property(x => x.CompletedAt)
            .IsRequired(false);

        builder.HasOne(x => x.Routine)
            .WithMany(r => r.Tasks)
            .HasForeignKey(x => x.RoutineId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}