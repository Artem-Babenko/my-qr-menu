using Microsoft.AspNetCore.Mvc;
using QrMenuAPI.APP.Mappings;
using QrMenuAPI.Core;
using QrMenuAPI.Core.Consts;

namespace QrMenuAPI.APP.Controllers;

[Route("user")]
public class UserController : BaseApiController
{
    readonly AppDbContext db;

    public UserController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet("")]
    public async Task<IActionResult> CurrentUser()
    {
        var userIdClaim = User.FindFirst(AuthConsts.USER_ID_CLAIM);
        if (string.IsNullOrEmpty(userIdClaim?.Value))
            return Unauthorized();

        var userId = int.Parse(userIdClaim.Value);
        var user = await db.Users.FindAsync(userId);
        if (user == null)
            return Unauthorized();

        var model = UserMapper.MapToModel(user);
        return Ok(model);
    }
}
