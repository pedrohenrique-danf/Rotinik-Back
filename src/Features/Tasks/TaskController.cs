using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Rotinik.Core.Extensions;
using Rotinik.Features.Tasks.DTOs;

namespace Rotinik.Features.Tasks;

[Authorize]
[Route("api/routine/{routineId}/task")]
[ApiController]
public class TaskController : ControllerBase
{
    private readonly TaskService _taskService;
    private readonly Rotinik.Features.Routines.RoutineService _routineService;

    public TaskController(TaskService taskService, Rotinik.Features.Routines.RoutineService routineService)
    {
        _taskService = taskService;
        _routineService = routineService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateTask(int routineId, TaskCreateDto dto)
    {
        var currentUserId = User.GetCurrentUserId();
        var result = await _taskService.CreateTaskAsync(routineId, currentUserId, dto);
        var updatedRoutine = await _routineService.GetRoutineByIdAsync(routineId, currentUserId);
        return StatusCode(201, updatedRoutine);
    }

    [HttpPut("{taskId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTask(int routineId, int taskId, TaskUpdateDto dto)
    {
        var currentUserId = User.GetCurrentUserId();
        await _taskService.UpdateTaskAsync(routineId, taskId, currentUserId, dto);
        var updatedRoutine = await _routineService.GetRoutineByIdAsync(routineId, currentUserId);
        return Ok(updatedRoutine);
    }

    [HttpDelete("{taskId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTask(int routineId, int taskId)
    {
        var currentUserId = User.GetCurrentUserId();
        await _taskService.DeleteTaskAsync(routineId, taskId, currentUserId);
        var updatedRoutine = await _routineService.GetRoutineByIdAsync(routineId, currentUserId);
        return Ok(updatedRoutine);
    }

    [HttpPatch("{taskId}/start")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> StartTask(int routineId, int taskId)
    {
        var currentUserId = User.GetCurrentUserId();
        await _taskService.StartTaskAsync(routineId, taskId, currentUserId);
        return Ok(new { message = "Task started successfully." });
    }

    [HttpPatch("{taskId}/toggle")]
    [EnableRateLimiting("TaskTogglePolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleCompletion(int routineId, int taskId)
    {
        var currentUserId = User.GetCurrentUserId();
        
        var unlockedMedals = await _taskService.ToggleTaskCompletionAsync(routineId, taskId, currentUserId);
        
        return Ok(new { data = new { newlyUnlockedMedals = unlockedMedals } });
    }
}