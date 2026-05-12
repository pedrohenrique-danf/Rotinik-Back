using System.ComponentModel.DataAnnotations;
using RotinikApi.Models.Enums;

namespace RotinikApi.DTOs.Requests.Task
{
    /// <summary>FMRT_4, FMRT_10, FMRT_11 — Payload to create a new task.</summary>
    public class TaskCreateRequest
    {
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        /// <summary>FMRT_10 — Expected duration in minutes (1–1440).</summary>
        [Required]
        [Range(1, 1440)]
        public int EstimatedMinutes { get; set; }

        /// <summary>FMRT_10 — Recurrence frequency.</summary>
        [Required]
        public TaskFrequency Frequency { get; set; }

        /// <summary>FMRT_11 — Task priority/type.</summary>
        [Required]
        public TaskType Type { get; set; }
    }
}
