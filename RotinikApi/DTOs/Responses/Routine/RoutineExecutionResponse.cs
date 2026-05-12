namespace RotinikApi.DTOs.Responses.Routine
{
    /// <summary>Response DTO for a routine execution record (FMRT_14).</summary>
    public class RoutineExecutionResponse
    {
        public int Id { get; set; }
        public int RoutineId { get; set; }
        public int UserId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public double ElapsedMinutes { get; set; }
        public bool IsFinished => FinishedAt.HasValue;
    }
}
