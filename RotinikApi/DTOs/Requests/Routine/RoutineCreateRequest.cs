using System.ComponentModel.DataAnnotations;

namespace RotinikApi.DTOs.Requests.Routine
{
    /// <summary>FMRT_1 — Payload to create a new user routine.</summary>
    public class RoutineCreateRequest
    {
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? Theme { get; set; }
    }
}
