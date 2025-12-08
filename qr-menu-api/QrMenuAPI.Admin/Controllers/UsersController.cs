using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QrMenuAPI.APP.Mappings;
using QrMenuAPI.Core;

namespace QrMenuAPI.APP.Controllers;

[Route("users")]
public class UsersController(AppDbContext db) : BaseApiController
{
    [HttpGet("current")]
    public async Task<IActionResult> CurrentUser()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var user = await db.Users.FindAsync(userId);
        if (user == null)
            return Unauthorized();

        var model = UserMapper.MapToModel(user);
        return Ok(model);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchUser([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return Ok(null);

        var normalizedQuery = query.Trim();

        var user = await db.Users.FirstOrDefaultAsync(
            user => user.Phone.Contains(normalizedQuery)
        );

        return Ok(user != null ? UserMapper.MapToModel(user) : null);
    }
}
