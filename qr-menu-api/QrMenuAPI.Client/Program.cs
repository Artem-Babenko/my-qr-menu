using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
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
var staticFilesRoot = builder.Configuration["STATIC_FILES_ROOT"] ?? Path.Combine(app.Environment.ContentRootPath, "static");
Directory.CreateDirectory(staticFilesRoot);

app.UseRouting();

var corsOrigins = builder.Configuration["CorsOrigins"]?
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    ?? new[] { "http://localhost:5174" };

app.UseCors(opt => opt
    .WithOrigins(corsOrigins)
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials());
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(staticFilesRoot),
    RequestPath = "/static"
});

app.MapControllers();

app.Run();
