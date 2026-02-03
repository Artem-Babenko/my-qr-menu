using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QrMenuAPI.Admin.Consts;
using QrMenuAPI.Admin.Models.Network;
using QrMenuAPI.Admin.Utils;
using QrMenuAPI.Core;
using QrMenuAPI.Core.Enums;

namespace QrMenuAPI.Admin.Controllers;

[Route("establishments")]
public class EstablishmentsController(AppDbContext db) : BaseApiController
{
    [HttpPut("{establishmentId:int}")]
    public async Task<IActionResult> UpdateEstablishment(
        [FromRoute] int establishmentId,
        [FromBody] UpdateEstablishmentRequest req)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (establishmentId <= 0 || req == null || !req.IsValid())
            return BadRequest(ErrorCodes.InvalidRequest);

        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return Unauthorized(ErrorCodes.UserNotFound);

        if (!user.NetworkId.HasValue)
            return NotFound(ErrorCodes.NetworkNotFound);

        var establishment = await db.Establishments
            .FirstOrDefaultAsync(e =>
                e.Id == establishmentId &&
                e.NetworkId == user.NetworkId.Value);
        if (establishment == null)
            return NotFound(ErrorCodes.EstablishmentNotFound);

        var canEdit = await PermissionUtils.HasAnyEstablishmentPermission(
            db,
            userId,
            establishmentId,
            [PermissionType.EstablishmentsUpdate]);
        if (!canEdit)
            return Forbidden(ErrorCodes.PermissionDenied);

        var newName = req.Name.Trim();
        if (await db.Establishments.AnyAsync(e =>
            e.Name == newName && e.Id != establishmentId))
            return Conflict(ErrorCodes.DuplicateEstablishment);

        establishment.Name = newName;
        establishment.Address = req.Address.Trim();

        await db.SaveChangesAsync();
        return Success();
    }

    [HttpDelete("{establishmentId:int}")]
    public async Task<IActionResult> DeleteEstablishment(
        [FromRoute] int establishmentId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (establishmentId <= 0)
            return BadRequest(ErrorCodes.InvalidRequest);

        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return Unauthorized(ErrorCodes.UserNotFound);

        if (!user.NetworkId.HasValue)
            return NotFound(ErrorCodes.NetworkNotFound);

        var establishment = await db.Establishments
            .FirstOrDefaultAsync(e =>
                e.Id == establishmentId &&
                e.NetworkId == user.NetworkId.Value);
        if (establishment == null)
            return NotFound(ErrorCodes.EstablishmentNotFound);

        var canDelete = await PermissionUtils.HasAnyEstablishmentPermission(
            db,
            userId,
            establishmentId,
            [PermissionType.EstablishmentsDelete]);
        if (!canDelete)
            return Forbidden(ErrorCodes.PermissionDenied);

        db.Establishments.Remove(establishment);
        await db.SaveChangesAsync();

        return Success();
    }
}

