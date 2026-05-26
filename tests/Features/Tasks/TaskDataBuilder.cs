using Rotinik.Features.Tasks;
using Rotinik.Features.Tasks.DTOs;

namespace Rotinik.Tests.Features.Tasks;

public static class TaskDataBuilder
{
    public static TaskCreateDto CreateValidTaskDto() => new()
    {
        Title = $"Test Task {Guid.NewGuid():N}",
        Description = "This is a valid test task description.",
        ExecutionTime = new TimeOnly(14, 30), // 2:30 PM
        Priority = TaskPriority.Important
    };

    public static TaskUpdateDto CreateValidUpdateDto() => new()
    {
        Title = "Updated Task Title",
        Description = "Updated description for the existing task."
    };
}