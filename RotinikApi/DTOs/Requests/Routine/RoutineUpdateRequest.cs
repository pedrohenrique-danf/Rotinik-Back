using System.ComponentModel.DataAnnotations;

namespace RotinikApi.DTOs.Requests.Routine
{
    /// <summary>FMRT_2 — Payload to update an existing routine. All fields are optional.</summary>
    public class RoutineUpdateRequest
    {
        [StringLength(150, MinimumLength = 3)]
        public string? Name { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? Theme { get; set; }
    }
}
