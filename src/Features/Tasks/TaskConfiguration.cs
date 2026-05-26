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
<<<<<<< HEAD:src/Features/Tasks/TaskItemConfiguration.cs
            .HasMaxLength(500)
            .HasDefaultValue(string.Empty);

        builder.Property(x => x.Frequency)
            .HasConversion<string>()
=======
            .HasMaxLength(TaskConstants.DescriptionMaxLength);

        builder.Property(x => x.ExecutionTime)
>>>>>>> 8f6767e86801caf4e023b326a294d23f55ee89ed:src/Features/Tasks/TaskConfiguration.cs
            .IsRequired();

        builder.Property(x => x.Priority)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.IsCompleted)
            .HasDefaultValue(false);

        builder.Property(x => x.CompletedAt)
            .IsRequired(false);

        builder.Property(x => x.XpReward)
            .IsRequired()
            .HasDefaultValue(10);

        builder.Property(x => x.CoinReward)
            .IsRequired()
            .HasDefaultValue(5);

        builder.Property(x => x.Order)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasOne(x => x.Routine)
            .WithMany(r => r.Tasks)
            .HasForeignKey(x => x.RoutineId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}