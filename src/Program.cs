using Rotinik.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDatabaseConfiguration(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddCorsConfiguration()
    .AddApplicationServices()
    .AddOpenApi();

var app = builder.Build();

app.UseApplicationMiddlewares();

app.Run();

public partial class Program { }