using System.Net;

namespace Rotinik.Core.Exceptions;

public class NotFoundException : BaseAppException
{
    public NotFoundException(string message) : base(message, HttpStatusCode.NotFound) { }
}