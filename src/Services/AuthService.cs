using Rotinik.Core.Exceptions;
using Rotinik.Data.Repositories;
using Rotinik.DTOs.User;

namespace Rotinik.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IUserRepository userRepository, ITokenService tokenService, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<string?> LoginAsync(UserLoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        
        if (user == null || !_passwordHasher.VerifyPassword(dto.Password, user.Password))
            throw new UnauthorizedException("Invalid email or password.");

        return _tokenService.GenerateJwtToken(user);
    }
}