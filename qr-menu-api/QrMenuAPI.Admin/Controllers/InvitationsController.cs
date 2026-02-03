using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QrMenuAPI.Admin.Consts;
using QrMenuAPI.Admin.Mappings;
using QrMenuAPI.Admin.Models.Invitation;
using QrMenuAPI.Admin.Services.Invitations;
using QrMenuAPI.Admin.Utils;
using QrMenuAPI.Core;
using QrMenuAPI.Core.Enums;

namespace QrMenuAPI.Admin.Controllers;

[Route("invitations")]
public class InvitationsController(
    AppDbContext db,
    IInvitationService invitationService) : BaseApiController
{
    [HttpPost("for-existing-user")]
    public async Task<IActionResult> ForExistingUser([FromBody] InvitationForExisting payload)
    {
        if (!TryGetUserId(out var currentUserId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (payload == null || !payload.IsValid())
            return BadRequest(ErrorCodes.InvalidRequest);

        var currentUser = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == currentUserId);
        if (currentUser == null)
            return Unauthorized(ErrorCodes.UserNotFound);
        if (!currentUser.NetworkId.HasValue)
            return NotFound(ErrorCodes.NetworkNotFound);

        var establishment = await db.Establishments
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == payload.EstablishmentId && e.NetworkId == currentUser.NetworkId.Value);
        if (establishment == null)
            return NotFound(ErrorCodes.EstablishmentNotFound);

        var canInvite = await PermissionUtils.HasEstablishmentPermission(
            db,
            currentUserId,
            payload.EstablishmentId,
            PermissionType.InvitationsCreate);
        if (!canInvite)
            return Forbidden(ErrorCodes.PermissionDenied);

        var roleExists = await db.Roles
            .AsNoTracking()
            .AnyAsync(r => r.Id == payload.RoleId && r.NetworkId == currentUser.NetworkId.Value);
        if (!roleExists)
            return NotFound(ErrorCodes.RoleNotFound);

        var user = await db.Users.FindAsync(payload.TargetUserId);
        if (user == null || user.NetworkId.HasValue)
            return NotFound(ErrorCodes.UserNotFound);

        var invitation = InvitationMapper.MapToEntity(payload);
        invitation.TargetUserId = payload.TargetUserId;

        db.Invitations.Add(invitation);
        await db.SaveChangesAsync();

        return Success(new { InvitationId = invitation.Id });
    }

    [HttpPost("for-new-user")]
    public async Task<IActionResult> ForNewUser([FromBody] InvitationForNew payload)
    {
        if (!TryGetUserId(out var currentUserId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (payload == null || !payload.IsValid())
            return BadRequest(ErrorCodes.InvalidRequest);

        var currentUser = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == currentUserId);
        if (currentUser == null)
            return Unauthorized(ErrorCodes.UserNotFound);
        if (!currentUser.NetworkId.HasValue)
            return NotFound(ErrorCodes.NetworkNotFound);

        var establishment = await db.Establishments
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == payload.EstablishmentId && e.NetworkId == currentUser.NetworkId.Value);
        if (establishment == null)
            return NotFound(ErrorCodes.EstablishmentNotFound);

        var canInvite = await PermissionUtils.HasEstablishmentPermission(
            db,
            currentUserId,
            payload.EstablishmentId,
            PermissionType.InvitationsCreate);
        if (!canInvite)
            return Forbidden(ErrorCodes.PermissionDenied);

        var roleExists = await db.Roles
            .AsNoTracking()
            .AnyAsync(r => r.Id == payload.RoleId && r.NetworkId == currentUser.NetworkId.Value);
        if (!roleExists)
            return NotFound(ErrorCodes.RoleNotFound);

        var invitation = InvitationMapper.MapToEntity(payload);
        invitation.Phone = payload.Phone;
        invitation.Name = payload.Name;
        invitation.Surname = payload.Surname;

        await db.Invitations.AddAsync(invitation);
        await db.SaveChangesAsync();

        return Success(new { InvitationId = invitation.Id });
    }

    [HttpGet("by-network/{networkId:int}")]
    public async Task<IActionResult> GetByNetwork([FromRoute] int networkId)
    {
        if (!TryGetUserId(out var currentUserId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (networkId <= 0)
            return BadRequest(ErrorCodes.InvalidRequest);

        var currentUser = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == currentUserId);
        if (currentUser == null)
            return Unauthorized(ErrorCodes.UserNotFound);
        if (!currentUser.NetworkId.HasValue || currentUser.NetworkId.Value != networkId)
            return NotFound(ErrorCodes.NetworkNotFound);

        var canViewInv = await PermissionUtils.HasAnyNetworkPermission(
            db,
            currentUserId,
            networkId,
            [PermissionType.InvitationsView]);
        if (!canViewInv)
            return Forbidden(ErrorCodes.PermissionDenied);

        var invitations = await db.Invitations
            .Include(inv => inv.TargetUser)
            .Where(inv => inv.Establishment.NetworkId == networkId)
            .ToListAsync();

        var models = invitations
            .Select(InvitationMapper.MapToModel)
            .ToList();

        return Success(models);
    }

    [AllowAnonymous]
    [HttpGet("{invitationId:Guid}")]
    public async Task<IActionResult> GetInvitation([FromRoute] Guid invitationId)
    {
        if (invitationId == Guid.Empty)
            return BadRequest(ErrorCodes.InvalidRequest);

        var invitation = await db.Invitations
            .Include(inv => inv.TargetUser)
            .Include(inv => inv.Role)
            .Include(inv => inv.Establishment)
            .FirstOrDefaultAsync(inv => inv.Id == invitationId);

        return Success(invitation?.MapToUserModel());
    }

    [HttpGet("by-current-user")]
    public async Task<IActionResult> GetByCurrentUser()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return NotFound(ErrorCodes.UserNotFound);

        var invitations = await db.Invitations
            .Include(inv => inv.Role)
            .Include(inv => inv.Establishment)
            .Include(inv => inv.TargetUser)
            .Where(inv =>
                (inv.TargetUserId == userId) ||
                (inv.Phone == user.Phone)
            )
            .ToListAsync();

        var models = invitations
            .Select(InvitationMapper.MapToUserModel)
            .ToList();

        return Success(models);
    }

    [HttpPost("{invitationId:Guid}/accept")]
    public async Task<IActionResult> AcceptInvitationForExistingUser([FromRoute] Guid invitationId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        var user = await db.Users.FindAsync(userId);
        if (user == null)
            return NotFound(ErrorCodes.UserNotFound);

        if (invitationId == Guid.Empty || user.NetworkId.HasValue)
            return BadRequest(ErrorCodes.InvalidRequest);

        var invitation = await db.Invitations
          .Include(inv => inv.Establishment)
          .FirstOrDefaultAsync(inv => inv.Id == invitationId);

        if (invitation == null)
            return NotFound(ErrorCodes.InvitationNotFound);

        var isTargetUserValid =
            invitation.TargetUserId == user.Id ||
            invitation.Phone == user.Phone;

        var isExpired = invitation.ExpiredAt <= DateTime.UtcNow;

        if (!isTargetUserValid || isExpired)
            return NotFound(ErrorCodes.InvitationNotFound);

        using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            await invitationService.AcceptInvitation(invitation, user);
            await transaction.CommitAsync();
            return Success(new { invitation.Establishment.NetworkId });
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    [HttpDelete("{invitationId:Guid}")]
    public async Task<IActionResult> DeleteInvitation([FromRoute] Guid invitationId)
    {
        if (!TryGetUserId(out var currentUserId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (invitationId == Guid.Empty)
            return BadRequest(ErrorCodes.InvalidRequest);

        var invitation = await db.Invitations
            .Include(i => i.Establishment)
            .FirstOrDefaultAsync(i => i.Id == invitationId);
        if (invitation == null)
            return NotFound(ErrorCodes.InvitationNotFound);

        var currentUser = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == currentUserId);
        if (currentUser == null)
            return Unauthorized(ErrorCodes.UserNotFound);
        if (!currentUser.NetworkId.HasValue || currentUser.NetworkId.Value != invitation.Establishment.NetworkId)
            return NotFound(ErrorCodes.NetworkNotFound);

        var canDelete = await PermissionUtils.HasAnyEstablishmentPermission(
            db,
            currentUserId,
            invitation.EstablishmentId,
            [PermissionType.InvitationsDelete]);
        if (!canDelete)
            return Forbidden(ErrorCodes.PermissionDenied);

        db.Invitations.Remove(invitation);
        await db.SaveChangesAsync();

        return Success();
    }

    [HttpPut("{invitationId:Guid}/cancel")]
    public async Task<IActionResult> CancelInvation([FromRoute] Guid invitationId)
    {
        if (!TryGetUserId(out var currentUserId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (invitationId == Guid.Empty)
            return BadRequest(ErrorCodes.InvalidRequest);

        var invitation = await db.Invitations
            .Include(i => i.Establishment)
            .FirstOrDefaultAsync(i => i.Id == invitationId);
        if (invitation == null)
            return NotFound(ErrorCodes.InvitationNotFound);

        var currentUser = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == currentUserId);
        if (currentUser == null)
            return Unauthorized(ErrorCodes.UserNotFound);
        if (!currentUser.NetworkId.HasValue || currentUser.NetworkId.Value != invitation.Establishment.NetworkId)
            return NotFound(ErrorCodes.NetworkNotFound);

        var canCancel = await PermissionUtils.HasAnyEstablishmentPermission(
            db,
            currentUserId,
            invitation.EstablishmentId,
            [PermissionType.InvitationsDelete]);
        if (!canCancel)
            return Forbidden(ErrorCodes.PermissionDenied);

        invitation.Status = InvitationStatus.Canceled;
        db.Invitations.Update(invitation);
        await db.SaveChangesAsync();

        return Success();
    }
}
