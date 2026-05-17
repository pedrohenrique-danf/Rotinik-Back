using System.Net;

namespace Rotinik.Core.Exceptions;

public class ForbiddenException : BaseAppException
{
    public ForbiddenException(string message) : base(message, HttpStatusCode.Forbidden) { }
}