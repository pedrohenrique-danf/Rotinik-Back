using Scalar.AspNetCore;
using Rotinik.Core.Middleware;

namespace Rotinik.Core.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseApplicationMiddlewares(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
            
            app.Lifetime.ApplicationStarted.Register(() =>
            {
                app.Logger.LogInformation("Scalar API Docs: http://localhost:5025/scalar");
            });
        }

        app.UseMiddleware<GlobalExceptionMiddleware>();

        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapControllers();

        return app;
    }
}