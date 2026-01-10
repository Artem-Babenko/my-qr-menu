using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QrMenuAPI.Admin.Consts;
using QrMenuAPI.Admin.Models.Roles;
using QrMenuAPI.Core;

namespace QrMenuAPI.Admin.Controllers;

[Route("roles")]
public class RolesController(AppDbContext db) : BaseApiController
{
    [HttpGet("{networkId:int}")]
    public async Task<IActionResult> GetRoles(int networkId)
    {
        if (networkId <= 0)
            return BadRequest(ErrorCodes.InvalidRequest);

        var roles = await db.Roles
            .Include(r => r.UserEstablishment)
            .Include(r => r.Permissions)
            .Where(r => r.NetworkId == networkId)
            .ToListAsync();

        var models = roles.Select(r => new RoleViewModel()
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            NumberOfUsers = r.UserEstablishment.Count,
            Permissions = [.. r.Permissions.Select(p => p.PermissionType)]
        }).ToList();

        return Success(models);
    }
}
