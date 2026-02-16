using QrMenuAPI.Admin.Utils;

var builder = WebApplication.CreateBuilder(args);

WebAppBuildHelper.AddOptions(builder);
WebAppBuildHelper.AddDbContext(builder);
WebAppBuildHelper.AddAuthentication(builder);
WebAppBuildHelper.AddServices(builder);

builder.Services.AddControllers();
builder.Services.AddCors();

var app = builder.Build();

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

WebAppHelper.UseCustomMiddlewares(app);
WebAppHelper.EnsureDbCreatedAndMigrated(app);

app.MapControllers();

app.Run();
