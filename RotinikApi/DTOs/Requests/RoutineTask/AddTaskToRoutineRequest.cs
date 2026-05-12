using System.ComponentModel.DataAnnotations;

namespace RotinikApi.DTOs.Requests.RoutineTask
{
    /// <summary>FMRT_7 — Payload to add an existing task into a routine.</summary>
    public class AddTaskToRoutineRequest
    {
        [Required]
        public int TaskId { get; set; }

        /// <summary>Display order within the routine (0-based).</summary>
        [Range(0, int.MaxValue)]
        public int Order { get; set; }
    }
}
