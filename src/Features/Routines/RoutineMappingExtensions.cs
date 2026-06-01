using Rotinik.Features.Routines.DTOs;
using Rotinik.Features.Tasks.DTOs;

namespace Rotinik.Features.Routines;

public static class RoutineMappingExtensions
{
    public static RoutineResponseDto ToResponseDto(this Routine routine)
    {
        return new RoutineResponseDto
        {
            Id = routine.Id,
            Title = routine.Title,
            Description = routine.Description,
            Category = routine.Category,
            Frequency = routine.Frequency,
            IsDefault = routine.IsDefault,
            CreatedAt = routine.CreatedAt,
            Tasks = routine.Tasks.Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IsCompleted = t.IsCompleted,
                XpReward = t.XpReward,
                CoinReward = t.CoinReward,
                Order = t.Order,
                CompletedAt = t.CompletedAt,
                RoutineId = t.RoutineId,
                EstimatedMinutes = t.EstimatedMinutes,
                Importance = t.Priority == Rotinik.Features.Tasks.TaskPriority.Low ? "baixa" :
                             t.Priority == Rotinik.Features.Tasks.TaskPriority.Moderate ? "media" :
                             t.Priority == Rotinik.Features.Tasks.TaskPriority.Important ? "alta" :
                             t.Priority == Rotinik.Features.Tasks.TaskPriority.Urgent ? "critica" : "media"
            }).OrderBy(t => t.Order).ToList()
        };
    }
}