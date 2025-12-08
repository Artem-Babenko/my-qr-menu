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

app.UseCors(opt => opt
    .WithOrigins("http://localhost:5173")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials());

app.UseAuthentication();
app.UseAuthorization();

WebAppHelper.UseCustomMiddlewares(app);
WebAppHelper.EnsureDbCreatedAndMigrated(app);

app.MapControllers();

app.Run();
