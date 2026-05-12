using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RotinikApi.DTOs.Requests.RoutineTask;
using RotinikApi.Services.RoutineTasks;

namespace RotinikApi.Controllers
{
    /// <summary>
    /// FMRT_7, FMRT_8, FMRT_9, FMRT_12, FMRT_14 — Manages tasks within a routine.
    /// </summary>
    [ApiController]
    [Route("api/routine/{routineId:int}/tasks")]
    [Authorize]
    public class RoutineTaskController : ControllerBase
    {
        private readonly IRoutineTaskService _service;

        public RoutineTaskController(IRoutineTaskService service)
        {
            _service = service;
        }

        /// <summary>FMRT_7 — Adds an existing task to a routine.</summary>
        [HttpPost]
        public async Task<IActionResult> AddTask(int routineId, [FromBody] AddTaskToRoutineRequest dto)
        {
            try
            {
                var result = await _service.AddTaskAsync(routineId, dto, GetUserId());
                return Ok(result);
            }
            catch (KeyNotFoundException ex)     { return NotFound(new { Message = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { Message = ex.Message }); }
        }

        /// <summary>FMRT_8 — Updates a task entry within a routine (e.g. reorder).</summary>
        [HttpPut("{routineTaskId:int}")]
        public async Task<IActionResult> UpdateRoutineTask(int routineId, int routineTaskId,
            [FromBody] UpdateRoutineTaskRequest dto)
        {
            try
            {
                var result = await _service.UpdateAsync(routineId, routineTaskId, dto, GetUserId());
                return Ok(result);
            }
            catch (KeyNotFoundException ex)     { return NotFound(new { Message = ex.Message }); }
            catch (ArgumentException ex)        { return BadRequest(new { Message = ex.Message }); }
        }

        /// <summary>FMRT_9 — Removes a task from a routine.</summary>
        [HttpDelete("{routineTaskId:int}")]
        public async Task<IActionResult> RemoveTask(int routineId, int routineTaskId)
        {
            try
            {
                await _service.RemoveAsync(routineId, routineTaskId, GetUserId());
                return NoContent();
            }
            catch (KeyNotFoundException ex) { return NotFound(new { Message = ex.Message }); }
        }

        /// <summary>FMRT_12 + FMRT_14 — Marks a task as completed, validating elapsed time.</summary>
        [HttpPatch("{routineTaskId:int}/complete")]
        public async Task<IActionResult> Complete(int routineId, int routineTaskId,
            [FromBody] CompleteRoutineTaskRequest dto)
        {
            try
            {
                var result = await _service.CompleteAsync(routineId, routineTaskId, dto, GetUserId());
                return Ok(result);
            }
            catch (KeyNotFoundException ex)     { return NotFound(new { Message = ex.Message }); }
            catch (InvalidOperationException ex) { return UnprocessableEntity(new { Message = ex.Message }); }
        }

        /// <summary>FMRT_12 — Undoes a task completion (marks as not completed).</summary>
        [HttpPatch("{routineTaskId:int}/uncomplete")]
        public async Task<IActionResult> Uncomplete(int routineId, int routineTaskId)
        {
            try
            {
                var result = await _service.UncompleteAsync(routineId, routineTaskId, GetUserId());
                return Ok(result);
            }
            catch (KeyNotFoundException ex)     { return NotFound(new { Message = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { Message = ex.Message }); }
        }

        // ── Helper ──────────────────────────────────────────────────────────
        private int GetUserId()
        {
            var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? User.FindFirst("sub")?.Value;

            if (sub is null || !int.TryParse(sub, out var id))
                throw new UnauthorizedAccessException("Invalid or missing token subject.");

            return id;
        }
    }
}
