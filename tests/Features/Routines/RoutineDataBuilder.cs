using Rotinik.Features.Routines.DTOs;

namespace Rotinik.Tests.Features.Routines;

public static class RoutineDataBuilder
{
    public static RoutineCreateDto CreateValidRoutineDto() => new()
    {
        Title = "Treino Matinal",
        Category = "Saúde"
    };

    public static RoutineUpdateDto CreateValidUpdateDto() => new()
    {
        Title = "Yoga Noturna",
        Category = "Bem-estar"
    };
}