using AutoMapper;
using Rotinik.DTOs.Routine;
using Rotinik.Models;

namespace Rotinik.Mappings;

public class RoutineMappingProfile : Profile
{
    public RoutineMappingProfile()
    {
        CreateMap<RoutineCreateDto, Routine>()
            .ForMember(dest => dest.IdUser, opt => opt.Ignore());
    }
}