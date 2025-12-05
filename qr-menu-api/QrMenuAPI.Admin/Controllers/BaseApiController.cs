using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QrMenuAPI.APP.Controllers;

[Authorize]
[ApiController]
public abstract class BaseApiController : Controller
{

}
