using Catalog.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var catalogDbConnectionString = builder.Configuration.GetConnectionString("CatalogDbConnectionString");

builder.Services.AddDbContext<CatalogDbContext>(options => options.UseNpgsql(catalogDbConnectionString));

var app = builder.Build();

app.Run();