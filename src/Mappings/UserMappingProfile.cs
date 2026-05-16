using AutoMapper;
using Rotinik.DTOs.User;
using Rotinik.Models;

namespace Rotinik.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        // Mapeamentos de Saída: Model (Banco) -> DTO (Resposta)
        CreateMap<User, UserProfileDto>();
        CreateMap<User, UserResponseDto>();

        // Mapeamento de Entrada: DTO (Requisição) -> Model (Banco)
        CreateMap<UserRegistrationDto, User>()
            // Ignoramos a senha aqui porque fazemos o Hash de forma segura no Service
            .ForMember(dest => dest.Password, opt => opt.Ignore())
            // Garantimos que a data vá como UTC para o banco
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate.ToUniversalTime()));
    }
}