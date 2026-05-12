using System.ComponentModel.DataAnnotations;
using RotinikApi.Models.Enums;

namespace RotinikApi.DTOs.Requests.Task
{
    /// <summary>FMRT_5 — Payload to update a task. All fields are optional.</summary>
    public class TaskUpdateRequest
    {
        [StringLength(200, MinimumLength = 3)]
        public string? Title { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Range(1, 1440)]
        public int? EstimatedMinutes { get; set; }

        public TaskFrequency? Frequency { get; set; }

        public TaskType? Type { get; set; }
    }
}
