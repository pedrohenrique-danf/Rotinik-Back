using AutoMapper;
using Rotinik.Core.Exceptions;
using Rotinik.Data.Repositories;
using Rotinik.DTOs.User;

namespace Rotinik.Services;

public class UserQueryService : IUserQueryService
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;

    public UserQueryService(IUserRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<UserProfileDto?> GetPublicProfileAsync(string username)
    {
        var user = await _repository.GetByUserNameAsync(username);
        if (user == null)  
            throw new NotFoundException("User not found.");

        return _mapper.Map<UserProfileDto>(user);
    }

    public async Task<UserResponseDto?> GetCurrentUserAsync(int userId)
    {
        var user = await _repository.GetByIdAsync(userId);
        if (user == null)  
            throw new NotFoundException("User not found.");

        return _mapper.Map<UserResponseDto>(user);
    }
}