using Scalar.AspNetCore;
using Rotinik.Core.Data;

namespace Rotinik.Core.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseApplicationMiddlewares(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.WithTitle("Rotinik API")
                .AddHttpAuthentication("Bearer", auth =>
                {
                    auth.Token = "";
                });
            });

            app.Lifetime.ApplicationStarted.Register(() =>
            {
                app.Logger.LogInformation("Scalar API Docs: http://localhost:5025/scalar");
            });

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
            }
        }

        app.UseExceptionHandler();
        app.UseRateLimiter();
        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}