using System.Net;
using System.Text.Json;
using Rotinik.Core.Exceptions;

namespace Rotinik.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Deixa a requisição seguir o fluxo normal
            await _next(context);
        }
        catch (Exception ex)
        {
            // Se der erro em qualquer lugar (Controller, Service, etc), cai aqui
            _logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Mapeia a exceção do Core para um Status Code HTTP
        context.Response.StatusCode = exception switch
        {
            NotFoundException => (int)HttpStatusCode.NotFound,
            ConflictException => (int)HttpStatusCode.Conflict,
            ForbiddenException => (int)HttpStatusCode.Forbidden,
            ValidationException => (int)HttpStatusCode.BadRequest,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            _ => (int)HttpStatusCode.InternalServerError // Erro genérico/inesperado (500)
        };

        var response = new { message = exception.Message };
        var payload = JsonSerializer.Serialize(response);

        return context.Response.WriteAsync(payload);
    }
}