using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QrMenuAPI.Admin.Consts;
using QrMenuAPI.Admin.Mappings;
using QrMenuAPI.Admin.Models.User;
using QrMenuAPI.Core;
using QrMenuAPI.Core.Entities;

namespace QrMenuAPI.Admin.Controllers;

[Route("users")]
public class UsersController(AppDbContext db) : BaseApiController
{
    [HttpGet("current")]
    public async Task<IActionResult> CurrentUser()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        var user = await db.Users
            .AsNoTracking()
            .Include(u => u.UserEstablishment)
            .FirstOrDefaultAsync(u => u.Id == userId);
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
            .AsNoTracking()
            .Include(u => u.UserEstablishment)
            .Where(u => u.NetworkId == networkId)
            .Select(u => u.MapToModel())
            .ToListAsync();

        return Success(users);
    }

    [HttpPut("{userId:int}/establishments")]
    public async Task<IActionResult> UpdateUserEstablishments(
        [FromRoute] int userId,
        [FromBody] UpdateUserEstablishmentsRequest req)
    {
        if (!TryGetUserId(out var currentUserId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (userId <= 0 || req == null)
            return BadRequest(ErrorCodes.InvalidRequest);

        var accesses = req.Accesses ?? [];
        if (accesses.Count == 0)
            return BadRequest(ErrorCodes.InvalidRequest);

        if (accesses.Any(x => x.EstablishmentId <= 0 || x.RoleId <= 0))
            return BadRequest(ErrorCodes.InvalidRequest);

        if (accesses.Select(x => x.EstablishmentId).Distinct().Count() != accesses.Count)
            return BadRequest(ErrorCodes.InvalidRequest);

        var currentUser = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == currentUserId);
        if (currentUser == null)
            return Unauthorized(ErrorCodes.UserNotFound);

        if (!currentUser.NetworkId.HasValue)
            return NotFound(ErrorCodes.NetworkNotFound);

        var networkId = currentUser.NetworkId.Value;

        var user = await db.Users
            .Include(u => u.UserEstablishment)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null || user.NetworkId != networkId)
            return NotFound(ErrorCodes.UserNotFound);

        var establishmentIds = accesses.Select(x => x.EstablishmentId).ToList();
        var validEstablishmentIds = await db.Establishments
            .AsNoTracking()
            .Where(e => e.NetworkId == networkId && establishmentIds.Contains(e.Id))
            .Select(e => e.Id)
            .ToListAsync();
        if (validEstablishmentIds.Count != establishmentIds.Count)
            return BadRequest(ErrorCodes.InvalidRequest);

        var roleIds = accesses.Select(x => x.RoleId).Distinct().ToList();
        var validRoleIds = await db.Roles
            .AsNoTracking()
            .Where(r => r.NetworkId == networkId && roleIds.Contains(r.Id))
            .Select(r => r.Id)
            .ToListAsync();
        if (validRoleIds.Count != roleIds.Count)
            return BadRequest(ErrorCodes.InvalidRequest);

        var requestedByEstablishmentId = accesses.ToDictionary(x => x.EstablishmentId, x => x.RoleId);

        var existing = user.UserEstablishment.ToList();
        foreach (var ue in existing)
        {
            if (!requestedByEstablishmentId.TryGetValue(ue.EstablishmentId, out var newRoleId))
            {
                db.UserEstablishments.Remove(ue);
                continue;
            }

            if (ue.RoleId != newRoleId)
                ue.RoleId = newRoleId;
        }

        var existingEstablishmentIds = existing.Select(x => x.EstablishmentId).ToHashSet();
        foreach (var (establishmentId, roleId) in requestedByEstablishmentId)
        {
            if (existingEstablishmentIds.Contains(establishmentId))
                continue;

            user.UserEstablishment.Add(new UserEstablishmentEntity
            {
                UserId = user.Id,
                EstablishmentId = establishmentId,
                RoleId = roleId
            });
        }

        await db.SaveChangesAsync();
        return Success();
    }
}
