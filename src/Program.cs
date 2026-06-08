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

app.Run();

public partial class Program { }