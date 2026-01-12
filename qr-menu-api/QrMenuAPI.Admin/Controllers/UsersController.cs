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
    public async Task<IActionResult> SearchUser([FromQuery] string phone)
    {
        if (string.IsNullOrWhiteSpace(phone) || phone.Length < 2)
            return Success(null);

        var normalizedQuery = phone.Trim();

        var user = await db.Users.FirstOrDefaultAsync(
            user => user.Phone.Contains(normalizedQuery)
        );

        return Success(user?.MapToModel());
    }

    [HttpGet("by-network/{networkId:int}")]
    public async Task<IActionResult> GetByNetwork(int networkId)
    {
        var users = await db.Users
            .Where(u => u.NetworkId == networkId)
            .Select(u => u.MapToModel())
            .ToListAsync();

        return Success(users);
    }
}
