using System.Security.Claims;
using System.Text;
using Auth.Application.BackgroundServices;
using Auth.Application.Features;
using Auth.Application.Settings;
using Auth.Domain;
using Auth.Infrastructure;
using Catalog.Application.Caching;
using Catalog.Application.Features.Restaurant.Command;
using Catalog.Application.Settings;
using Catalog.Application.Validation;
using Catalog.Infrastructure;
using Catalog.Infrastructure.Caching;
using FluentValidation;
using Geo.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using Web.Middlewares.Exceptions;
using Role = Auth.Domain.Role;

namespace Web.Extensions;

public static class AppBuilderExtensions
{
    public static IServiceCollection AddMyCustomMiddlewares(this IServiceCollection services)
    {
        services.AddExceptionHandler<UserNameIsAlreadyInUseExceptionHandler>();
        services.AddExceptionHandler<EmailIsAlreadyInUseExceptionHandler>();
        services.AddExceptionHandler<FailedAddUserRoleExceptionHandler>();
        services.AddExceptionHandler<UserCreateFailedExceptionHandler>();
        services.AddExceptionHandler<UnauthorizedExceptionHandler>();
        services.AddExceptionHandler<BadRequestExceptionHandler>();
        services.AddExceptionHandler<NotFoundExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        
        return services;
    }
    
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnectionString");

        services.AddDbContext<CatalogDbContext>(options =>
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());

        services.AddDbContext<AuthDbContext>(options =>
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());

        services.AddDbContext<GeoDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.UseNetTopologySuite();
                })
                .UseSnakeCaseNamingConvention();
        });

        services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<AuthDbContext>();

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => 
        {
            cfg.RegisterServicesFromAssemblyContaining<CreateRestaurantCommand>();
            cfg.RegisterServicesFromAssemblyContaining<UserRegisterCommand>();
            
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(CachingBehavior<,>));
        });

        services.AddValidatorsFromAssemblyContaining<CreateRestaurantCommand>();
        
        services.AddHostedService<RefreshTokenCleaner>();
        
        return services;
    }

    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddProblemDetails();
        
        return services;
    }

    public static IServiceCollection AddMyCustomConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<CacheSettings>(configuration.GetSection("CacheSettings"));
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        
        return services;
    }
    
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var connectionString = configuration.GetConnectionString("Redis") ?? "localhost:6379";
            return ConnectionMultiplexer.Connect(connectionString);
        });

        services.AddScoped<RedisCacheService>();

        return services;
    }
    
    public static IServiceCollection AddAuthDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnectionString");
        
        services.AddDbContext<AuthDbContext>(options =>
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());

        services.AddScoped<TokenProvider>();

        return services;
    }
    
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection("JwtSettings");
        var secretKey = jwtSection["Secret"]!;
        var issuer = jwtSection["Issuer"];
        var audience = jwtSection["Audience"];

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuer = !string.IsNullOrEmpty(issuer),
                    ValidIssuer = issuer,
                    ValidateAudience = !string.IsNullOrEmpty(audience),
                    ValidAudience = audience,
                    ClockSkew = TimeSpan.Zero,
            
                    RoleClaimType = ClaimTypes.Role 
                };
            });

        services.AddAuthorization();

        return services;
    }
}