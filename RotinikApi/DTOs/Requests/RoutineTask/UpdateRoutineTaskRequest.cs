using System.ComponentModel.DataAnnotations;

namespace RotinikApi.DTOs.Requests.RoutineTask
{
    /// <summary>FMRT_8 — Payload to update a task entry within a routine (order only).</summary>
    public class UpdateRoutineTaskRequest
    {
        [Range(0, int.MaxValue)]
        public int? Order { get; set; }
    }
}
