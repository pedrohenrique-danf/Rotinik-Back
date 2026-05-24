using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Rotinik.Core.Exceptions;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is BaseAppException appException)
        {
            logger.LogWarning("Business rule triggered: {Message}", exception.Message);
            httpContext.Response.StatusCode = (int)appException.StatusCode;
        }
        else
        {
            logger.LogError(exception, "An unhandled exception has occurred.");
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }

        var problemDetails = new ProblemDetails
        {
            Status = httpContext.Response.StatusCode,
            Title = exception is BaseAppException ? "Business Rule Violation" : "Internal Server Error",
            Detail = exception is BaseAppException ? exception.Message : "An unexpected error occurred."
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}