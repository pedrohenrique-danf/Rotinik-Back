using System.ComponentModel.DataAnnotations;

namespace RotinikApi.DTOs.Requests.RoutineTask
{
    /// <summary>FMRT_12, FMRT_14 — Payload to mark a routine task as completed.</summary>
    public class CompleteRoutineTaskRequest
    {
        /// <summary>
        /// The ID of the active RoutineExecution.
        /// Required so the system can validate elapsed time (FMRT_14).
        /// </summary>
        [Required]
        public int RoutineExecutionId { get; set; }
    }
}
