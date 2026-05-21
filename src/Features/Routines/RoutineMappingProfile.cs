using AutoMapper;
using Rotinik.Features.Routines.DTO;

namespace Rotinik.Features.Routines;

public class RoutineMappingProfile : Profile
{
    public RoutineMappingProfile()
    {
        CreateMap<RoutineCreateDto, Routine>()
            .ForMember(dest => dest.IdUser, opt => opt.Ignore());
            
        CreateMap<Routine, RoutineResponseDto>();
    }
}