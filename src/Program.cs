using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Rotinik.Settings;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// DATABASE CONFIGURATION
// ==========================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<Rotinik.Data.AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// ==========================================
// OPTIONS PATTERN CONFIGURATION
// ==========================================
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// ==========================================
// AUTHENTICATION CONFIGURATION (JWT)
// ==========================================
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var jwtSecret = jwtSettings?.Secret ?? "TemporaryKeySoEFCoreMigrationDoesNotBreak!";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidAudience = jwtSettings?.Audience,
            ValidIssuer = jwtSettings?.Issuer,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddAutoMapper(config => 
{
    config.AddMaps(typeof(Program).Assembly);
});

builder.Services.AddScoped<Rotinik.Data.Repositories.IUserRepository, Rotinik.Data.Repositories.UserRepository>();
builder.Services.AddScoped<Rotinik.Services.ITokenService, Rotinik.Services.TokenService>();
builder.Services.AddScoped<Rotinik.Services.IUserCommandService, Rotinik.Services.UserCommandService>();
builder.Services.AddScoped<Rotinik.Services.IUserQueryService, Rotinik.Services.UserQueryService>();
builder.Services.AddScoped<Rotinik.Services.IAuthService, Rotinik.Services.AuthService>();
builder.Services.AddSingleton<Rotinik.Services.IPasswordHasher, Rotinik.Services.PasswordHasher>();

// ==========================================
// OPENAPI & SCALAR CONFIGURATION
// ==========================================
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// ==========================================
// MIDDLEWARE PIPELINE
// ==========================================
app.UseMiddleware<Rotinik.Middleware.GlobalExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ==========================================
// CUSTOM CONSOLE MESSAGES
// ==========================================
if (app.Environment.IsDevelopment())
{
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        app.Logger.LogInformation("Scalar API Docs: http://localhost:5206/scalar");
    });
}

app.Run();

public partial class Program { }