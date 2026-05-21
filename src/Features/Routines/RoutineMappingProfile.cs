using AutoMapper;

namespace Rotinik.Features.Routines;

public class RoutineMappingProfile : Profile
{
    public RoutineMappingProfile()
    {
        CreateMap<RoutineCreateDto, Routine>()
            .ForMember(dest => dest.IdUser, opt => opt.Ignore());
    }
}