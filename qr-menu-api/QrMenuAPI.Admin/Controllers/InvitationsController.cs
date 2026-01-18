using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QrMenuAPI.Admin.Consts;
using QrMenuAPI.Admin.Mappings;
using QrMenuAPI.Admin.Models.Invitation;
using QrMenuAPI.Admin.Services.Invitations;
using QrMenuAPI.Core;

namespace QrMenuAPI.Admin.Controllers;

[Route("invitations")]
public class InvitationsController(
    AppDbContext db,
    IInvitationService invitationService) : BaseApiController
{
    [HttpPost("for-existing-user")]
    public async Task<IActionResult> ForExistingUser([FromBody] InvitationForExisting payload)
    {
        if (payload == null || !payload.IsValid())
            return BadRequest(ErrorCodes.InvalidRequest);

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
        if (payload == null || !payload.IsValid())
            return BadRequest(ErrorCodes.InvalidRequest);

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
        if (networkId <= 0)
            return BadRequest(ErrorCodes.InvalidRequest);

        var invitations = await db.Invitations
            .Include(inv => inv.TargetUser)
            .Where(inv => inv.Establishment.NetworkId == networkId)
            .ToListAsync();

        var models = invitations
            .Select(InvitationMapper.MapToModel)
            .ToList();

        return Success(models);
    }

    [HttpGet("{invitationId:Guid}")]
    public async Task<IActionResult> GetInvitation([FromRoute] Guid invitationId)
    {
        if (invitationId == Guid.Empty)
            return BadRequest(ErrorCodes.InvalidRequest);

        var invitation = await db.Invitations
            .Include(inv => inv.TargetUser)
            .FirstOrDefaultAsync(inv => inv.Id == invitationId);

        return Success(invitation?.MapToModel());
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
            .Where(inv =>
                (inv.TargetUserId == userId) ||
                (inv.Phone == user.Phone)
            )
            .ToListAsync();

        var models = invitations
            .Select(InvitationMapper.MapToModel)
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

        if (invitation == null ||
            invitation.TargetUserId != userId ||
            invitation.ExpiredAt <= DateTime.UtcNow)
        {
            return NotFound(ErrorCodes.InvitationNotFound);
        }

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
        if (invitationId == Guid.Empty)
            return BadRequest(ErrorCodes.InvalidRequest);

        var invitation = await db.Invitations.FindAsync(invitationId);
        if (invitation == null)
            return NotFound(ErrorCodes.InvitationNotFound);

        db.Invitations.Remove(invitation);
        await db.SaveChangesAsync();

        return Success();
    }
}
