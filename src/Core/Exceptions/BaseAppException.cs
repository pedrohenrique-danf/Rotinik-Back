using System.Net;

namespace Rotinik.Core.Exceptions;

public abstract class BaseAppException : Exception
{
    public HttpStatusCode StatusCode { get; }

    protected BaseAppException(string message, HttpStatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}