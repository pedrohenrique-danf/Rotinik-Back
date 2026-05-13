using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RotinikApi.Data;
using RotinikApi.Services;
using RotinikApi.Services.Routines;
using RotinikApi.Services.Tasks;
using RotinikApi.Services.RoutineTasks;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddOpenApi();

// CORS: Configuração robusta para permitir o tráfego do Angular (localhost:4200)
builder.Services.AddCors(options =>
{
    options.AddPolicy("RotinikAppPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Permite o envio seguro de cookies/tokens se necessário
    });
});

// Database
builder.Services.AddDbContext<RotinikContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("RotinikConnection"))
);

// JWT: Read settings from appsettings
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var jwtKey = jwtSettings["Key"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = jwtSettings["Issuer"],
            ValidAudience            = jwtSettings["Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoutineService, RoutineService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IRoutineTaskService, RoutineTaskService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "RotinikApi";
        options.Theme = ScalarTheme.DeepSpace;
        options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

// Em ambiente de desenvolvimento local (HTTP), o Redirection às vezes causa problemas de CORS no Preflight do Angular.
// Se estiver testando estritamente em http://localhost:5025, você pode comentar a linha abaixo.
app.UseHttpsRedirection();

// UseCors DEVE vir antes de Authentication e Authorization
app.UseCors("RotinikAppPolicy");

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();