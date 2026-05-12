using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RotinikApi.DTOs.Requests.Routine;
using RotinikApi.Services.Routines;

namespace RotinikApi.Controllers
{
    /// <summary>
    /// FMRT_1, FMRT_2, FMRT_3, FMRT_13, FMRT_14 — Manages user routines.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RoutineController : ControllerBase
    {
        private readonly IRoutineService _service;

        public RoutineController(IRoutineService service)
        {
            _service = service;
        }

        /// <summary>Lists all routines belonging to the authenticated user.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var routines = await _service.GetAllAsync(GetUserId());
            return Ok(routines);
        }

        /// <summary>Returns a single routine with its task list.</summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var routine = await _service.GetByIdAsync(id, GetUserId());
            return routine is null
                ? NotFound(new { Message = "Routine not found." })
                : Ok(routine);
        }

        /// <summary>FMRT_1 — Creates a new routine.</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoutineCreateRequest dto)
        {
            var routine = await _service.CreateAsync(dto, GetUserId());
            return CreatedAtAction(nameof(GetById), new { id = routine.Id }, routine);
        }

        /// <summary>FMRT_2 — Updates an existing routine.</summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] RoutineUpdateRequest dto)
        {
            try
            {
                var routine = await _service.UpdateAsync(id, dto, GetUserId());
                return Ok(routine);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        /// <summary>FMRT_3 — Deletes a routine.</summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id, GetUserId());
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        // ── FMRT_13 — Templates ─────────────────────────────────────────────

        /// <summary>FMRT_13 — Lists all pre-made template routines.</summary>
        [HttpGet("templates")]
        public async Task<IActionResult> GetTemplates()
        {
            var templates = await _service.GetTemplatesAsync();
            return Ok(templates);
        }

        /// <summary>FMRT_13 — Clones a template routine into the user's account.</summary>
        [HttpPost("templates/{templateId:int}/clone")]
        public async Task<IActionResult> CloneTemplate(int templateId)
        {
            try
            {
                var routine = await _service.CloneTemplateAsync(templateId, GetUserId());
                return CreatedAtAction(nameof(GetById), new { id = routine.Id }, routine);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        // ── FMRT_14 — Execution tracking ────────────────────────────────────

        /// <summary>FMRT_14 — Starts a routine execution session (required before completing tasks).</summary>
        [HttpPost("{id:int}/executions")]
        public async Task<IActionResult> StartExecution(int id)
        {
            try
            {
                var execution = await _service.StartExecutionAsync(id, GetUserId());
                return Ok(execution);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        /// <summary>FMRT_14 — Finishes a routine execution session.</summary>
        [HttpPatch("{id:int}/executions/{executionId:int}/finish")]
        public async Task<IActionResult> FinishExecution(int id, int executionId)
        {
            try
            {
                var execution = await _service.FinishExecutionAsync(executionId, GetUserId());
                return Ok(execution);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
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
