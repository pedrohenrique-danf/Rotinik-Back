namespace RotinikApi.Models
{
    /// <summary>
    /// FMRT_14 — Tracks when a user started and finished executing a routine.
    /// Used to validate whether enough time has elapsed before allowing task completion.
    /// </summary>
    public class RoutineExecution
    {
        public int Id { get; protected set; }

        public int RoutineId { get; protected set; }
        public int UserId { get; protected set; }

        /// <summary>UTC timestamp when the user started this routine execution.</summary>
        public DateTime StartedAt { get; protected set; }

        /// <summary>UTC timestamp when the execution was marked finished. Null if still in progress.</summary>
        public DateTime? FinishedAt { get; protected set; }

        // Navigation
        public Routine Routine { get; protected set; } = null!;
        public User User { get; protected set; } = null!;

        protected RoutineExecution() { }

        public RoutineExecution(int routineId, int userId)
        {
            RoutineId = routineId;
            UserId    = userId;
            StartedAt = DateTime.UtcNow;
        }

        /// <summary>Marks the execution as finished.</summary>
        public void Finish()
        {
            if (FinishedAt.HasValue)
                throw new InvalidOperationException("Execution is already finished.");

            FinishedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// FMRT_14 — Returns the total minutes elapsed since this execution started.
        /// </summary>
        public double ElapsedMinutes() => (DateTime.UtcNow - StartedAt).TotalMinutes;
    }
}
