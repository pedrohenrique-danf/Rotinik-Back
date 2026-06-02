using System.Net;

namespace Rotinik.Core.Exceptions;

public class BadRequestException : BaseAppException
{
    public BadRequestException(string message) : base(message, HttpStatusCode.BadRequest) { }
}