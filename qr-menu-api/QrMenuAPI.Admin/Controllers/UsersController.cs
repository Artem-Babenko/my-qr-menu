using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QrMenuAPI.Admin.Consts;
using QrMenuAPI.Admin.Mappings;
using QrMenuAPI.Core;

namespace QrMenuAPI.Admin.Controllers;

[Route("users")]
public class UsersController(AppDbContext db) : BaseApiController
{
    [HttpGet("current")]
    public async Task<IActionResult> CurrentUser()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        var user = await db.Users.FindAsync(userId);
        if (user == null)
            return Unauthorized(ErrorCodes.UserNotFound);

        var model = user.MapToModel();
        return Success(model);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchUser([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return Success(null);

        var normalizedQuery = query.Trim();

        var user = await db.Users.FirstOrDefaultAsync(
            user => user.Phone.Contains(normalizedQuery)
        );

        return Success(user?.MapToModel());
    }
}
