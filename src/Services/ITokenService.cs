using System.Security.Claims;
using Rotinik.Models;

namespace Rotinik.Services;

public interface ITokenService
{
    string GenerateJwtToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}