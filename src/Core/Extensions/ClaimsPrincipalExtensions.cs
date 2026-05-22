using System.Security.Claims;
using Rotinik.Core.Exceptions;

namespace Rotinik.Core.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetCurrentUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
            throw new UnauthorizedException("User ID claim is missing or invalid.");
        
        return userId;
    }
}