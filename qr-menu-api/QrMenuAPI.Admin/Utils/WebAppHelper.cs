using Microsoft.EntityFrameworkCore;
using QrMenuAPI.Admin.Middlewares;
using QrMenuAPI.Core;

namespace QrMenuAPI.Admin.Utils;

public class WebAppHelper
{
    public static void EnsureDbCreatedAndMigrated(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }

    public static void UseCustomMiddlewares(WebApplication app)
    {
        app.UseMiddleware<UserActivityMiddleware>();
    }
}
