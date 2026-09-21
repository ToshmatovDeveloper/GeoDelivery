using Serilog;
using Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services
    .AddMyCustomMiddlewares()
    .AddDatabase(builder.Configuration)
    .AddApplication()
    .AddPresentation()
    .AddMyCustomConfiguration(builder.Configuration)
    .AddRedis(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();