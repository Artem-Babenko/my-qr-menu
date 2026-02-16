using Microsoft.AspNetCore.Mvc;
using QrMenuAPI.Client.Models;

namespace QrMenuAPI.Client.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    public IActionResult Success() =>
        Ok(new ApiResponse());

    public IActionResult Success(object? data) =>
        Ok(new ApiResponse(data));

    public IActionResult BadRequest(string errorCode, object? data = null) =>
        BadRequest(new ApiResponse(errorCode, data));

    public IActionResult NotFound(string errorCode, object? data = null) =>
        base.NotFound(new ApiResponse(errorCode, data));

    public IActionResult Conflict(string errorCode, object? data = null) =>
        Conflict(new ApiResponse(errorCode, data));
}
