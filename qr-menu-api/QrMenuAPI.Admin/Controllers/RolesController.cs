using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QrMenuAPI.Admin.Consts;
using QrMenuAPI.Admin.Models.Roles;
using QrMenuAPI.Admin.Utils;
using QrMenuAPI.Core;
using QrMenuAPI.Core.Entities;
using QrMenuAPI.Core.Enums;

namespace QrMenuAPI.Admin.Controllers;

[Route("roles")]
public class RolesController(AppDbContext db) : BaseApiController
{
    [HttpGet("{networkId:int}")]
    public async Task<IActionResult> GetRoles(int networkId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (networkId <= 0)
            return BadRequest(ErrorCodes.InvalidRequest);

        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return Unauthorized(ErrorCodes.UserNotFound);

        if (!user.NetworkId.HasValue || user.NetworkId.Value != networkId)
            return NotFound(ErrorCodes.NetworkNotFound);

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

    [HttpPost("create")]
    public async Task<IActionResult> CreateRole([FromBody] RoleRequest req)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (!req.IsValid())
            return BadRequest(ErrorCodes.InvalidRequest);

        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return Unauthorized(ErrorCodes.UserNotFound);
        if (!user.NetworkId.HasValue || user.NetworkId.Value != req.NetworkId)
            return NotFound(ErrorCodes.NetworkNotFound);

        var canCreateRoles = await PermissionUtils.HasAnyNetworkPermission(
            db,
            userId,
            req.NetworkId,
            [PermissionType.RolesCreate]);
        if (!canCreateRoles)
            return Forbidden(ErrorCodes.PermissionDenied);

        if (!await db.Networks.AnyAsync(n => n.Id == req.NetworkId))
            return NotFound(ErrorCodes.NetworkNotFound);

        if (await db.Roles.AnyAsync(r => r.NetworkId == req.NetworkId && r.Name == req.Name))
            return Conflict(ErrorCodes.RoleDuplicateName);

        var role = new RoleEntity
        {
            Name = req.Name,
            Description = req.Description,
            NetworkId = req.NetworkId,
            Permissions = req.Permissions
                .Distinct()
                .Select(p => new RolePermissionEntity { PermissionType = p })
                .ToList()
        };

        db.Roles.Add(role);
        await db.SaveChangesAsync();

        var model = new RoleViewModel
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            NumberOfUsers = 0,
            Permissions = [.. role.Permissions.Select(p => p.PermissionType)]
        };

        return Success(model);
    }

    [HttpPut("{roleId:int}")]
    public async Task<IActionResult> UpdateRole(int roleId, [FromBody] RoleRequest req)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (roleId <= 0 || !req.IsValid())
            return BadRequest(ErrorCodes.InvalidRequest);

        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return Unauthorized(ErrorCodes.UserNotFound);
        if (!user.NetworkId.HasValue || user.NetworkId.Value != req.NetworkId)
            return NotFound(ErrorCodes.NetworkNotFound);

        var canEditRoles = await PermissionUtils.HasAnyNetworkPermission(
            db,
            userId,
            req.NetworkId,
            [PermissionType.RolesCreate]);
        if (!canEditRoles)
            return Forbidden(ErrorCodes.PermissionDenied);

        var role = await db.Roles
            .Include(r => r.Permissions)
            .Include(r => r.UserEstablishment)
            .FirstOrDefaultAsync(r => r.Id == roleId && r.NetworkId == req.NetworkId);

        if (role == null)
            return NotFound(ErrorCodes.RoleNotFound);

        if (await db.Roles.AnyAsync(r => r.NetworkId == req.NetworkId && r.Name == req.Name && r.Id != roleId))
            return Conflict(ErrorCodes.RoleDuplicateName);

        role.Name = req.Name;
        role.Description = req.Description;

        db.RolePermissions.RemoveRange(role.Permissions);
        role.Permissions.Clear();
        foreach (var permissionType in req.Permissions.Distinct())
        {
            role.Permissions.Add(new RolePermissionEntity { PermissionType = permissionType });
        }

        await db.SaveChangesAsync();

        var model = new RoleViewModel
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            NumberOfUsers = role.UserEstablishment.Count,
            Permissions = [.. role.Permissions.Select(p => p.PermissionType)]
        };

        return Success(model);
    }

    [HttpDelete("{roleId:int}")]
    public async Task<IActionResult> DeleteRole(int roleId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (roleId <= 0)
            return BadRequest(ErrorCodes.InvalidRequest);

        var role = await db.Roles
            .Include(r => r.UserEstablishment)
            .FirstOrDefaultAsync(r => r.Id == roleId);

        if (role == null)
            return NotFound(ErrorCodes.RoleNotFound);

        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return Unauthorized(ErrorCodes.UserNotFound);
        if (!user.NetworkId.HasValue || user.NetworkId.Value != role.NetworkId)
            return NotFound(ErrorCodes.NetworkNotFound);

        var canDeleteRoles = await PermissionUtils.HasAnyNetworkPermission(
            db,
            userId,
            role.NetworkId,
            [PermissionType.RolesDelete]);
        if (!canDeleteRoles)
            return Forbidden(ErrorCodes.PermissionDenied);

        if (role.Name == DefaultRoles.Owner)
            return Conflict(ErrorCodes.RoleDeleteForbidden);

        if (role.UserEstablishment.Any())
            return Conflict(ErrorCodes.RoleHasUsers);

        db.Roles.Remove(role);
        await db.SaveChangesAsync();

        return Success();
    }
}
