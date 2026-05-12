namespace RotinikApi.Models
{
    /// <summary>
    /// FMRT_7, FMRT_8, FMRT_9, FMRT_12 — Join entity linking a UserTask to a Routine.
    /// Tracks per-routine completion state for each task.
    /// </summary>
    public class RoutineTask
    {
        public int Id { get; protected set; }

        public int RoutineId { get; protected set; }
        public int TaskId { get; protected set; }

        /// <summary>Display order of the task within the routine.</summary>
        public int Order { get; protected set; }

        /// <summary>FMRT_12 — Whether this task has been completed in its routine context.</summary>
        public bool IsCompleted { get; protected set; }

        /// <summary>FMRT_12 / FMRT_14 — Timestamp when the task was marked complete.</summary>
        public DateTime? CompletedAt { get; protected set; }

        // Navigation
        public Routine Routine { get; protected set; } = null!;
        public UserTask Task { get; protected set; } = null!;

        protected RoutineTask() { }

        public RoutineTask(int routineId, int taskId, int order)
        {
            RoutineId   = routineId;
            TaskId      = taskId;
            SetOrder(order);
            IsCompleted = false;
            CompletedAt = null;
        }

        public void SetOrder(int order)
        {
            if (order < 0)
                throw new ArgumentException("Task order must be a non-negative integer.");

            Order = order;
        }

        /// <summary>
        /// FMRT_12 + FMRT_14 — Marks the task as completed.
        /// Caller is responsible for validating elapsed time before calling this.
        /// </summary>
        public void Complete()
        {
            if (IsCompleted)
                throw new InvalidOperationException("Task is already marked as completed.");

            IsCompleted = true;
            CompletedAt = DateTime.UtcNow;
        }

        /// <summary>Resets completion state (undo).</summary>
        public void Uncomplete()
        {
            IsCompleted = false;
            CompletedAt = null;
        }
    }
}
