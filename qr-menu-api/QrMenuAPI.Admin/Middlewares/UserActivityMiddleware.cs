using QrMenuAPI.Admin.Consts;
using QrMenuAPI.Core;

namespace QrMenuAPI.Admin.Middlewares;

public class UserActivityMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context, AppDbContext db)
    {
        var sessionId = context.User.FindFirst(AuthConsts.SESSION_ID_CLAIM)?.Value;

        if (sessionId != null && Guid.TryParse(sessionId, out var sid))
        {
            var session = await db.UserSessions.FindAsync(sid);
            if (session != null && session.ExpiresAt > DateTime.UtcNow)
            {
                session.LastActivityAt = DateTime.UtcNow;
                await db.SaveChangesAsync();
            }
        }

        await next(context);
    }
}
