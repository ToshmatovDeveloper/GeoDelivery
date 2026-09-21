using Catalog.Application.Features.Restaurant.Command;
using Catalog.Application.Validation;
using Catalog.Infrastructure;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Web.Middlewares.Exceptions;

namespace Web.Extensions;

public static class AppBuilderExtensions
{
    public static IServiceCollection AddMyCustomMiddlewares(this IServiceCollection services)
    {
        services.AddExceptionHandler<BadRequestExceptionHandler>();
        services.AddExceptionHandler<NotFoundExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        
        return services;
    }
    
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CatalogDbConnectionString");
        services.AddDbContext<CatalogDbContext>(options =>
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());
        
        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => 
        {
            cfg.RegisterServicesFromAssemblyContaining<CreateRestaurantCommand>();
            
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssemblyContaining<CreateRestaurantCommand>();
        
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
}