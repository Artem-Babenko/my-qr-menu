using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QrMenuAPI.Admin.Models.Options;
using QrMenuAPI.Admin.Services.Invitations;
using QrMenuAPI.Core;
using System.Text;

namespace QrMenuAPI.Admin.Utils;

public class WebAppBuildHelper
{
    public static void AddServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IInvitationService, InvitationService>();
    }

    public static void AddAuthentication(WebApplicationBuilder builder)
    {
        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var key = builder.Configuration["JwtSecret"]
                    ?? throw new Exception("Jwt key not found!");

                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    LifetimeValidator = (nbf, exp, token, param) =>
                    {
                        if (exp == null) return false;
                        return DateTime.UtcNow < exp.Value;
                    },
                };
            });
    }

    public static void AddDbContext(WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options
                .UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
                )
                .UseSnakeCaseNamingConvention();
        });
    }

    public static void AddOptions(WebApplicationBuilder builder)
    {
        builder.Services.Configure<AppSettings>(builder.Configuration);
    }
}
