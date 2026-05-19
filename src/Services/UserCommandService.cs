using AutoMapper;
using Rotinik.Core.Exceptions;
using Rotinik.Data.Repositories;
using Rotinik.DTOs.User;
using Rotinik.Models;

namespace Rotinik.Services;

public class UserCommandService : IUserCommandService
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;

    public UserCommandService(IUserRepository repository, IMapper mapper, IPasswordHasher passwordHasher)
    {
        _repository = repository;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
    }

    public async Task CreateUserAsync(UserRegistrationDto dto)
    {
        if (await _repository.GetByUserNameAsync(dto.UserName) != null)
            throw new ConflictException("UserName in use.");

        if (await _repository.GetByEmailAsync(dto.Email) != null)
            throw new ConflictException("Email in use.");

        var user = _mapper.Map<User>(dto);
        
        user.Password = _passwordHasher.HashPassword(dto.Password);  

        await _repository.AddAsync(user);
        await _repository.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(int id, int currentUserId, UserUpdateDto dto)
    {
        if (currentUserId != id)
            throw new ForbiddenException("Forbidden: You can only update your own profile.");

        var user = await _repository.GetByIdAsync(id);
        if (user == null)
            throw new NotFoundException("User not found.");

        user.Name = dto.Name;
        user.BirthDate = dto.BirthDate.ToUniversalTime();

        if (!string.IsNullOrEmpty(dto.Password))
        {
            user.Password = _passwordHasher.HashPassword(dto.Password);
        }

        await _repository.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int id, int currentUserId)
    {
        if (currentUserId != id)
            throw new ForbiddenException("Forbidden: You can only delete your own profile.");

        var user = await _repository.GetByIdAsync(id);
        if (user == null)
            throw new NotFoundException("User not found.");

        _repository.Remove(user);
        await _repository.SaveChangesAsync();
    }
}