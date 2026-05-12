using RotinikApi.Models.Enums;

namespace RotinikApi.Models
{
    /// <summary>
    /// FMRT_4, FMRT_10, FMRT_11 — A standalone task that can be added to one or more routines.
    /// Named UserTask internally to avoid conflicts with System.Threading.Tasks.Task.
    /// </summary>
    public class UserTask
    {
        public int Id { get; protected set; }

        /// <summary>Owner of this task. Null for system/template tasks.</summary>
        public int? UserId { get; protected set; }

        public string Title { get; protected set; } = string.Empty;
        public string? Description { get; protected set; }

        /// <summary>FMRT_10 — Minimum expected duration in minutes.</summary>
        public int EstimatedMinutes { get; protected set; }

        /// <summary>FMRT_10 — How often this task recurs.</summary>
        public TaskFrequency Frequency { get; protected set; }

        /// <summary>FMRT_11 — Classification/priority type.</summary>
        public TaskType Type { get; protected set; }

        public DateTime CreatedAt { get; protected set; }

        // Navigation
        public User? User { get; protected set; }
        public ICollection<RoutineTask> RoutineTasks { get; protected set; } = new List<RoutineTask>();

        protected UserTask() { }

        public UserTask(int? userId, string title, string? description, int estimatedMinutes,
                        TaskFrequency frequency, TaskType type)
        {
            UserId = userId;
            SetTitle(title);
            SetDescription(description);
            SetEstimatedMinutes(estimatedMinutes);
            Frequency  = frequency;
            Type       = type;
            CreatedAt  = DateTime.UtcNow;
        }

        public void SetTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Task title cannot be empty.");

            if (title.Length < 3 || title.Length > 200)
                throw new ArgumentException("Task title must be between 3 and 200 characters.");

            Title = title.Trim();
        }

        public void SetDescription(string? description)
        {
            if (description != null && description.Length > 1000)
                throw new ArgumentException("Task description cannot exceed 1000 characters.");

            Description = description?.Trim();
        }

        public void SetEstimatedMinutes(int minutes)
        {
            if (minutes < 1)
                throw new ArgumentException("Estimated duration must be at least 1 minute.");

            if (minutes > 1440)
                throw new ArgumentException("Estimated duration cannot exceed 1440 minutes (24 hours).");

            EstimatedMinutes = minutes;
        }

        public void SetFrequency(TaskFrequency frequency) => Frequency = frequency;
        public void SetType(TaskType type) => Type = type;
    }
}
