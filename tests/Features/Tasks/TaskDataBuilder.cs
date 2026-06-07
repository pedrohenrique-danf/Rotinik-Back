using Rotinik.Features.Tasks;
using Rotinik.Features.Tasks.DTOs;

namespace Rotinik.Tests.Features.Tasks;

public static class TaskDataBuilder
{
    public static TaskCreateDto CreateValidTaskDto() => new()
    {
        Title = $"Test Task {Guid.NewGuid():N}",
        Frequency = TaskFrequency.Daily,
        Importance = "alta"
    };

    public static TaskUpdateDto CreateValidUpdateDto() => new()
    {
        Title = "Updated Task Title",
        Frequency = TaskFrequency.Monthly,
        Importance = "critica"
    };
}