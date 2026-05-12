using RotinikApi.Models.Enums;

namespace RotinikApi.DTOs.Responses.RoutineTask
{
    /// <summary>Response DTO for a task entry within a routine.</summary>
    public class RoutineTaskResponse
    {
        public int Id { get; set; }
        public int RoutineId { get; set; }
        public int TaskId { get; set; }
        public string TaskTitle { get; set; } = string.Empty;
        public string? TaskDescription { get; set; }
        public int EstimatedMinutes { get; set; }
        public TaskFrequency Frequency { get; set; }
        public string FrequencyLabel => Frequency.ToString();
        public TaskType Type { get; set; }
        public string TypeLabel => Type.ToString();
        public int Order { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
