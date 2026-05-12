using RotinikApi.Models.Enums;

namespace RotinikApi.DTOs.Responses.Task
{
    /// <summary>Response DTO for a task.</summary>
    public class TaskResponse
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int EstimatedMinutes { get; set; }
        public TaskFrequency Frequency { get; set; }
        public string FrequencyLabel => Frequency.ToString();
        public TaskType Type { get; set; }
        public string TypeLabel => Type.ToString();
        public DateTime CreatedAt { get; set; }
    }
}
