using AutoMapper;
using Rotinik.DTOs.User;
using Rotinik.Models;

namespace Rotinik.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserProfileDto>();
        CreateMap<User, UserResponseDto>();

        CreateMap<UserRegistrationDto, User>()
            .ForMember(dest => dest.Password, opt => opt.Ignore())
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate.ToUniversalTime()));
    }
}