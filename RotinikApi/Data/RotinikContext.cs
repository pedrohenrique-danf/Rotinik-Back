using Microsoft.EntityFrameworkCore;
using RotinikApi.Models;
using RotinikApi.Models.Enums;

namespace RotinikApi.Data
{
    public class RotinikContext : DbContext
    {
        public RotinikContext(DbContextOptions<RotinikContext> opts) : base(opts) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Routine> Routines { get; set; }
        public DbSet<UserTask> Tasks { get; set; }
        public DbSet<RoutineTask> RoutineTasks { get; set; }
        public DbSet<RoutineExecution> RoutineExecutions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── User ──────────────────────────────────────────────────────────
            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(u => u.Id);
                e.HasIndex(u => u.Email).IsUnique();
                e.Property(u => u.Name).HasMaxLength(150).IsRequired();
                e.Property(u => u.Email).HasMaxLength(150).IsRequired();
                e.Property(u => u.Phone).HasMaxLength(20).IsRequired();
                e.Property(u => u.Password).HasMaxLength(256).IsRequired();
            });

            // ── Routine ───────────────────────────────────────────────────────
            modelBuilder.Entity<Routine>(e =>
            {
                e.HasKey(r => r.Id);
                e.Property(r => r.Name).HasMaxLength(150).IsRequired();
                e.Property(r => r.Description).HasMaxLength(1000);
                e.Property(r => r.Theme).HasMaxLength(100);

                e.HasOne(r => r.User)
                 .WithMany()
                 .HasForeignKey(r => r.UserId)
                 .OnDelete(DeleteBehavior.Cascade)
                 .IsRequired(false);
            });

            // ── UserTask ──────────────────────────────────────────────────────
            modelBuilder.Entity<UserTask>(e =>
            {
                e.ToTable("Tasks");
                e.HasKey(t => t.Id);
                e.Property(t => t.Title).HasMaxLength(200).IsRequired();
                e.Property(t => t.Description).HasMaxLength(1000);
                e.Property(t => t.Frequency).HasConversion<int>();
                e.Property(t => t.Type).HasConversion<int>();

                e.HasOne(t => t.User)
                 .WithMany()
                 .HasForeignKey(t => t.UserId)
                 .OnDelete(DeleteBehavior.Cascade)
                 .IsRequired(false);
            });

            // ── RoutineTask ───────────────────────────────────────────────────
            modelBuilder.Entity<RoutineTask>(e =>
            {
                e.HasKey(rt => rt.Id);

                e.HasOne(rt => rt.Routine)
                 .WithMany(r => r.RoutineTasks)
                 .HasForeignKey(rt => rt.RoutineId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(rt => rt.Task)
                 .WithMany(t => t.RoutineTasks)
                 .HasForeignKey(rt => rt.TaskId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ── RoutineExecution ──────────────────────────────────────────────
            modelBuilder.Entity<RoutineExecution>(e =>
            {
                e.HasKey(re => re.Id);

                e.HasOne(re => re.Routine)
                 .WithMany(r => r.Executions)
                 .HasForeignKey(re => re.RoutineId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(re => re.User)
                 .WithMany()
                 .HasForeignKey(re => re.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ── Seed: Pre-made template routines (FMRT_13) ───────────────────
            SeedTemplates(modelBuilder);
        }

        private static void SeedTemplates(ModelBuilder mb)
        {
            var now = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Template routines
            mb.Entity<Routine>().HasData(
                CreateTemplate(1, "Morning Routine",   "Start your day with clarity and energy.",     "Morning", now),
                CreateTemplate(2, "Fitness Routine",   "Stay active and build healthy habits.",        "Fitness", now),
                CreateTemplate(3, "Study Routine",     "Sharpen your focus and learn effectively.",    "Study",   now),
                CreateTemplate(4, "Evening Wind-Down", "Relax and prepare for restful sleep.",         "Evening", now)
            );

            // Template tasks
            mb.Entity<UserTask>().HasData(
                // Morning
                CreateTask(1,  null, "Drink a glass of water",    "Hydrate right after waking up.",          2,  TaskFrequency.Daily, TaskType.Important, now),
                CreateTask(2,  null, "10-minute stretching",       "Gentle full-body stretch.",                10, TaskFrequency.Daily, TaskType.Moderate,  now),
                CreateTask(3,  null, "Review daily goals",         "Write down your top 3 goals.",             5,  TaskFrequency.Daily, TaskType.Important, now),
                // Fitness
                CreateTask(4,  null, "Warm-up run",                "Light jog or brisk walk.",                 15, TaskFrequency.Daily, TaskType.Important, now),
                CreateTask(5,  null, "Strength training",          "Focus on one muscle group.",               45, TaskFrequency.Daily, TaskType.Urgent,    now),
                CreateTask(6,  null, "Post-workout stretch",       "Cool down and improve flexibility.",       10, TaskFrequency.Daily, TaskType.Moderate,  now),
                // Study
                CreateTask(7,  null, "Review yesterday's notes",   "Reinforce memory through recall.",        15, TaskFrequency.Daily, TaskType.Important, now),
                CreateTask(8,  null, "Deep-focus study block",     "No distractions, full focus.",            50, TaskFrequency.Daily, TaskType.Urgent,    now),
                CreateTask(9,  null, "Practice problems",          "Apply what you learned.",                 30, TaskFrequency.Daily, TaskType.Moderate,  now),
                // Evening
                CreateTask(10, null, "Reflect on the day",         "Write 3 wins and 1 improvement.",         10, TaskFrequency.Daily, TaskType.Moderate,  now),
                CreateTask(11, null, "Read for 20 minutes",        "Unplug and read something enriching.",    20, TaskFrequency.Daily, TaskType.Normal,    now),
                CreateTask(12, null, "Prepare tomorrow's plan",    "Set intentions for the next day.",        10, TaskFrequency.Daily, TaskType.Important, now)
            );

            // Link tasks to template routines
            mb.Entity<RoutineTask>().HasData(
                new { Id = 1,  RoutineId = 1, TaskId = 1,  Order = 0, IsCompleted = false, CompletedAt = (DateTime?)null },
                new { Id = 2,  RoutineId = 1, TaskId = 2,  Order = 1, IsCompleted = false, CompletedAt = (DateTime?)null },
                new { Id = 3,  RoutineId = 1, TaskId = 3,  Order = 2, IsCompleted = false, CompletedAt = (DateTime?)null },
                new { Id = 4,  RoutineId = 2, TaskId = 4,  Order = 0, IsCompleted = false, CompletedAt = (DateTime?)null },
                new { Id = 5,  RoutineId = 2, TaskId = 5,  Order = 1, IsCompleted = false, CompletedAt = (DateTime?)null },
                new { Id = 6,  RoutineId = 2, TaskId = 6,  Order = 2, IsCompleted = false, CompletedAt = (DateTime?)null },
                new { Id = 7,  RoutineId = 3, TaskId = 7,  Order = 0, IsCompleted = false, CompletedAt = (DateTime?)null },
                new { Id = 8,  RoutineId = 3, TaskId = 8,  Order = 1, IsCompleted = false, CompletedAt = (DateTime?)null },
                new { Id = 9,  RoutineId = 3, TaskId = 9,  Order = 2, IsCompleted = false, CompletedAt = (DateTime?)null },
                new { Id = 10, RoutineId = 4, TaskId = 10, Order = 0, IsCompleted = false, CompletedAt = (DateTime?)null },
                new { Id = 11, RoutineId = 4, TaskId = 11, Order = 1, IsCompleted = false, CompletedAt = (DateTime?)null },
                new { Id = 12, RoutineId = 4, TaskId = 12, Order = 2, IsCompleted = false, CompletedAt = (DateTime?)null }
            );
        }

        private static object CreateTemplate(int id, string name, string description, string theme, DateTime now)
            => new { Id = id, UserId = (int?)null, Name = name, Description = description, Theme = theme, IsTemplate = true, CreatedAt = now };

        private static object CreateTask(int id, int? userId, string title, string? description,
            int estimatedMinutes, TaskFrequency frequency, TaskType type, DateTime now)
            => new { Id = id, UserId = userId, Title = title, Description = description,
                     EstimatedMinutes = estimatedMinutes, Frequency = frequency, Type = type, CreatedAt = now };
    }
}