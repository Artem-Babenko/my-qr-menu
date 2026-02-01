using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QrMenuAPI.Admin.Consts;
using QrMenuAPI.Admin.Models.Api;

namespace QrMenuAPI.Admin.Controllers;

[Authorize]
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    public bool TryGetUserId(out int userId)
    {
        userId = 0;

        var userIdClaim = User.FindFirst(AuthConsts.USER_ID_CLAIM);
        if (string.IsNullOrEmpty(userIdClaim?.Value))
            return false;

        return int.TryParse(userIdClaim.Value, out userId);
    }

    public IActionResult Success() =>
        Ok(new ApiResponse());

    public IActionResult Success(object? data) =>
        Ok(new ApiResponse(data));

    public IActionResult BadRequest(string errorCode, object? data = null) =>
        BadRequest(new ApiResponse(errorCode, data));

    public IActionResult NotFound(string errorCode, object? data = null) =>
        NotFound(new ApiResponse(errorCode, data));

    public IActionResult Unauthorized(string errorCode, object? data = null) =>
        Unauthorized(new ApiResponse(errorCode, data));

    public IActionResult Conflict(string errorCode, object? data = null) =>
        Conflict(new ApiResponse(errorCode, data));

    public IActionResult Forbidden(string errorCode, object? data = null) =>
        StatusCode(StatusCodes.Status403Forbidden, new ApiResponse(errorCode, data));
}
