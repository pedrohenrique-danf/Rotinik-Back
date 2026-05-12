namespace RotinikApi.Models
{
    /// <summary>
    /// FMRT_1, FMRT_2, FMRT_3, FMRT_13 — A named collection of tasks.
    /// When IsTemplate is true, the routine is a pre-made, system-provided routine (FMRT_13).
    /// </summary>
    public class Routine
    {
        public int Id { get; protected set; }

        /// <summary>Null for template routines; set for user-owned routines.</summary>
        public int? UserId { get; protected set; }

        public string Name { get; protected set; }
        public string? Description { get; protected set; }

        /// <summary>FMRT_13 — Thematic category (e.g. "Morning", "Fitness", "Study").</summary>
        public string? Theme { get; protected set; }

        /// <summary>FMRT_13 — True when this is a system-provided template routine.</summary>
        public bool IsTemplate { get; protected set; }

        public DateTime CreatedAt { get; protected set; }

        // Navigation
        public User? User { get; protected set; }
        public ICollection<RoutineTask> RoutineTasks { get; protected set; } = new List<RoutineTask>();
        public ICollection<RoutineExecution> Executions { get; protected set; } = new List<RoutineExecution>();

        protected Routine() { }

        public Routine(int? userId, string name, string? description, string? theme, bool isTemplate = false)
        {
            UserId     = userId;
            IsTemplate = isTemplate;
            SetName(name);
            SetDescription(description);
            SetTheme(theme);
            CreatedAt  = DateTime.UtcNow;
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Routine name cannot be empty.");

            if (name.Length < 3 || name.Length > 150)
                throw new ArgumentException("Routine name must be between 3 and 150 characters.");

            Name = name.Trim();
        }

        public void SetDescription(string? description)
        {
            if (description != null && description.Length > 1000)
                throw new ArgumentException("Description cannot exceed 1000 characters.");

            Description = description?.Trim();
        }

        public void SetTheme(string? theme)
        {
            if (theme != null && theme.Length > 100)
                throw new ArgumentException("Theme cannot exceed 100 characters.");

            Theme = theme?.Trim();
        }
    }
}
