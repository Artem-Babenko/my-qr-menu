using Microsoft.EntityFrameworkCore;
using QrMenuAPI.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options
        .UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
        )
        .UseSnakeCaseNamingConvention();
});

builder.Services.AddControllers();
builder.Services.AddCors();

var app = builder.Build();

app.UseRouting();

var corsOrigins = builder.Configuration["CorsOrigins"]?
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    ?? new[] { "http://localhost:5174" };

app.UseCors(opt => opt
    .WithOrigins(corsOrigins)
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials());

app.MapControllers();

app.Run();
