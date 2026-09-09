using Catalog.Application.Features.Restaurant.Command;
using Catalog.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Serilog;

Serilog.Debugging.SelfLog.Enable(Console.Error);

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

var catalogDbConnectionString = builder.Configuration.GetConnectionString("CatalogDbConnectionString");

builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseNpgsql(catalogDbConnectionString)
        .UseSnakeCaseNamingConvention()); 

builder.Services.AddMediatR (cfg => cfg
        .RegisterServicesFromAssemblyContaining<CreateRestaurantCommand>());

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();