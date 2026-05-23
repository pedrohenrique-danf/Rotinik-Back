using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotinik.Core.Extensions;
using Rotinik.Features.Tasks.DTO;

namespace Rotinik.Features.Tasks;

[Authorize]
[Route("api/routine/{routineId}/task")]
[ApiController]
public class TaskController : ControllerBase
{
    private readonly TaskService _taskService;

    public TaskController(TaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(int routineId, TaskCreateDto dto)
    {
        var currentUserId = User.GetCurrentUserId();
        var result = await _taskService.CreateTaskAsync(routineId, currentUserId, dto);
        return StatusCode(201, new { data = result, message = "Task created." });
    }

    [HttpPut("{taskId}")]
    public async Task<IActionResult> UpdateTask(int routineId, int taskId, TaskUpdateDto dto)
    {
        var currentUserId = User.GetCurrentUserId();
        await _taskService.UpdateTaskAsync(routineId, taskId, currentUserId, dto);
        return NoContent();
    }

    [HttpDelete("{taskId}")]
    public async Task<IActionResult> DeleteTask(int routineId, int taskId)
    {
        var currentUserId = User.GetCurrentUserId();
        await _taskService.DeleteTaskAsync(routineId, taskId, currentUserId);
        return NoContent();
    }

    [HttpPatch("{taskId}/toggle")]
    public async Task<IActionResult> ToggleCompletion(int routineId, int taskId)
    {
        var currentUserId = User.GetCurrentUserId();
        await _taskService.ToggleTaskCompletionAsync(routineId, taskId, currentUserId);
        return NoContent();
    }
}