using Rotinik.Core.Extensions;
using Rotinik.Features.Users.Workers; // Importação limpa e direta

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDatabaseConfiguration(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddCorsConfiguration()
    .AddRateLimitingConfiguration()
    .AddApplicationServices()
    .AddOpenApi()
    .AddHostedService<AccountDeletionWorker>();

var app = builder.Build();

app.UseApplicationMiddlewares();

// Removed debug code

app.Run();
// Trigger clean reload after kill

public partial class Program { }