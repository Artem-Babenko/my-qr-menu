using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QrMenuAPI.Core.Consts;

namespace QrMenuAPI.APP.Controllers;

[Authorize]
[ApiController]
public abstract class BaseApiController : Controller
{
    public bool TryGetUserId(out int userId)
    {
        userId = 0;

        var userIdClaim = User.FindFirst(AuthConsts.USER_ID_CLAIM);
        if (string.IsNullOrEmpty(userIdClaim?.Value))
            return false;

        return int.TryParse(userIdClaim.Value, out userId);
    }
}
