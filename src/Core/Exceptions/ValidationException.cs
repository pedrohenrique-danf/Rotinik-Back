using System.Net;

namespace Rotinik.Core.Exceptions;

public class ValidationException : BaseAppException
{
    public ValidationException(string message) : base(message, HttpStatusCode.BadRequest) { }
}