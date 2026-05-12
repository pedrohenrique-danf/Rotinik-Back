using RotinikApi.DTOs.Responses.RoutineTask;

namespace RotinikApi.DTOs.Responses.Routine
{
    /// <summary>Full routine detail including ordered task list.</summary>
    public class RoutineDetailResponse
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Theme { get; set; }
        public bool IsTemplate { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<RoutineTaskResponse> Tasks { get; set; } = new();
    }
}
