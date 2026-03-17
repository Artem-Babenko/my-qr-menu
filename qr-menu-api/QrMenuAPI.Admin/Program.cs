using Microsoft.Extensions.FileProviders;
using QrMenuAPI.Admin.Utils;

var builder = WebApplication.CreateBuilder(args);

WebAppBuildHelper.AddOptions(builder);
WebAppBuildHelper.AddDbContext(builder);
WebAppBuildHelper.AddAuthentication(builder);
WebAppBuildHelper.AddServices(builder);

builder.Services.AddControllers();
builder.Services.AddCors();

var app = builder.Build();
var staticFilesRoot = builder.Configuration["STATIC_FILES_ROOT"]
    ?? Path.Combine(app.Environment.ContentRootPath, "static");
Directory.CreateDirectory(staticFilesRoot);

app.UseRouting();

var corsOrigins = builder.Configuration["CorsOrigins"]?
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    ?? new[] { "http://localhost:5173" };

app.UseCors(opt => opt
    .WithOrigins(corsOrigins)
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials());

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(staticFilesRoot),
    RequestPath = "/static"
});

WebAppHelper.UseCustomMiddlewares(app);
WebAppHelper.EnsureDbCreatedAndMigrated(app);

app.MapControllers();

app.Run();
