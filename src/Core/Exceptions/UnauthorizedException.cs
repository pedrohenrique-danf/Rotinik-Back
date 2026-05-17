using System.Net;

namespace Rotinik.Core.Exceptions;

public class UnauthorizedException : BaseAppException
{
    public UnauthorizedException(string message) : base(message, HttpStatusCode.Unauthorized) { }
}