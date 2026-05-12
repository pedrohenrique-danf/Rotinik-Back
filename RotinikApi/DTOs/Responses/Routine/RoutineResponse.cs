namespace RotinikApi.DTOs.Responses.Routine
{
    /// <summary>Lightweight routine summary (list views).</summary>
    public class RoutineResponse
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Theme { get; set; }
        public bool IsTemplate { get; set; }
        public int TaskCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
