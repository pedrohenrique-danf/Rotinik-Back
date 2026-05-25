using System.Text;
using System.Security.Claims;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Rotinik.Core.Data;
using Rotinik.Core.Settings;
using Rotinik.Core.Exceptions;

namespace Rotinik.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("RotinikConnection");
        
        services.AddDbContext<AppDbContext>(options => 
            options.UseNpgsql(connectionString, npgsqlOptions => 
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
            }));
        
        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
        
        if (string.IsNullOrWhiteSpace(jwtSettings?.Secret))
            throw new InvalidOperationException("JWT Secret is missing in appsettings.json.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ValidAudience = jwtSettings.Audience,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("PremiumOnly", policy => 
                policy.RequireClaim("isPremium", "True"));
        });
        
        return services;
    }

    public static IServiceCollection AddRateLimitingConfiguration(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                var partitionKey = httpContext.User.Identity?.IsAuthenticated == true 
                    ? httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                    : httpContext.Connection.RemoteIpAddress?.ToString();

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey ?? "anonymous",
                    partition => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 500,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    });
            });

            options.AddFixedWindowLimiter("LoginPolicy", opt =>
            {
                opt.Window = TimeSpan.FromMinutes(1);
                opt.PermitLimit = 500;
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 0; 
            });
        });

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddControllers();
        
        services.AddMemoryCache(); 
        
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<Program>();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        var assembly = typeof(Program).Assembly;
        var serviceTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Service"));

        foreach (var type in serviceTypes)
        {
            services.AddScoped(type);
        }

        var hasherType = assembly.GetTypes()
            .FirstOrDefault(t => t.Name == "PasswordHasher");
            
        if (hasherType != null)
        {
            services.AddScoped(hasherType);
        }

        return services;
    }

    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        return services;
    }
}