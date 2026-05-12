using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RotinikApi.DTOs.Requests.Task;
using RotinikApi.Services.Tasks;

namespace RotinikApi.Controllers
{
    /// <summary>
    /// FMRT_4, FMRT_5, FMRT_6, FMRT_10, FMRT_11 — Manages standalone tasks.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _service;

        public TaskController(ITaskService service)
        {
            _service = service;
        }

        /// <summary>Lists all tasks belonging to the authenticated user.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tasks = await _service.GetAllAsync(GetUserId());
            return Ok(tasks);
        }

        /// <summary>Returns a single task by ID.</summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var task = await _service.GetByIdAsync(id, GetUserId());
            return task is null
                ? NotFound(new { Message = "Task not found." })
                : Ok(task);
        }

        /// <summary>FMRT_4, FMRT_10, FMRT_11 — Creates a new task with frequency and type.</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TaskCreateRequest dto)
        {
            try
            {
                var task = await _service.CreateAsync(dto, GetUserId());
                return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>FMRT_5 — Updates a task. All fields are optional.</summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] TaskUpdateRequest dto)
        {
            try
            {
                var task = await _service.UpdateAsync(id, dto, GetUserId());
                return Ok(task);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>FMRT_6 — Deletes a task.</summary>
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
