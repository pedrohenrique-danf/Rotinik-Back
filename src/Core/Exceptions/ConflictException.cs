using System.Net;

namespace Rotinik.Core.Exceptions;

public class ConflictException : BaseAppException
{
    public ConflictException(string message) : base(message, HttpStatusCode.Conflict) { }
}