using Rotinik.Models;

namespace Rotinik.Services;

public interface ITokenService
{
    string GenerateJwtToken(User user);
}