using Rotinik.Features.Routines.DTOs;

namespace Rotinik.Features.Routines;

public static class RoutineMappingExtensions
{
    public static RoutineResponseDto ToResponseDto(this Routine routine)
    {
        return new RoutineResponseDto
        {
            Id = routine.Id,
            Title = routine.Title,
            Category = routine.Category,
            IsDefault = routine.IsDefault
        };
    }
}