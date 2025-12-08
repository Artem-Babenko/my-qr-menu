using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QrMenuAPI.Admin.Mappings;
using QrMenuAPI.Admin.Models.Invitation;
using QrMenuAPI.APP.Controllers;
using QrMenuAPI.Core;
using QrMenuAPI.Core.Entities;
using QrMenuAPI.Core.Enums;

namespace QrMenuAPI.Admin.Controllers;

[Route("invitations")]
public class InvitationsController(AppDbContext db) : BaseApiController
{
    [HttpPost("for-existing-user")]
    public async Task<IActionResult> ForExistingUser(InvitationForExisting payload)
    {
        if (payload == null || !payload.IsValid())
            return BadRequest();

        var user = await db.Users.FindAsync(payload.TargetUserId);
        if (user == null || user.NetworkId.HasValue)
            return BadRequest();

        var invitation = InvitationMapper.MapToEntity(payload);
        invitation.TargetUserId = payload.TargetUserId;

        db.Invitations.Add(invitation);
        await db.SaveChangesAsync();

        return Ok(new { InvitationId = invitation.Id });
    }

    [HttpPost("for-new-user")]
    public async Task<IActionResult> ForNewUser(InvitationForNew payload)
    {
        if (payload == null || !payload.IsValid())
            return BadRequest();

        var invitation = InvitationMapper.MapToEntity(payload);
        invitation.Phone = payload.Phone;
        invitation.Name = payload.Name;
        invitation.Surname = payload.Surname;

        await db.Invitations.AddAsync(invitation);
        await db.SaveChangesAsync();

        return Ok(new { InvitationId = invitation.Id });
    }

    [HttpGet("by-establishment/{establishmentId:int}")]
    public async Task<IActionResult> GetByEstablishment(int establishmentId)
    {
        if (establishmentId <= 0)
            return BadRequest();

        var invitations = await db.Invitations
            .Include(inv => inv.TargetUser)
            .Where(inv => inv.EstablishmentId == establishmentId)
            .ToListAsync();

        var models = invitations
            .Select(InvitationMapper.MapToModel)
            .ToList();

        return Ok(models);
    }

    [HttpGet("{invitationId:Guid}")]
    public async Task<IActionResult> GetInvitation(Guid invitationId)
    {
        if (invitationId == Guid.Empty)
            return BadRequest();

        var invitation = await db.Invitations
            .Include(inv => inv.TargetUser)
            .FirstOrDefaultAsync(inv => inv.Id == invitationId);

        return Ok(invitation != null
            ? InvitationMapper.MapToModel(invitation)
            : null);
    }

    [HttpGet("by-current-user")]
    public async Task<IActionResult> GetByCurrentUser()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return Unauthorized();

        var invitations = await db.Invitations
            .Where(inv =>
                (inv.TargetUserId == userId) ||
                (inv.Phone == user.Phone)
            )
            .ToListAsync();

        var models = invitations
            .Select(InvitationMapper.MapToModel)
            .ToList();

        return Ok(models);
    }

    [HttpPost("{invitationId:Guid}/accept")]
    public async Task<IActionResult> AcceptInvitationForExistingUser(Guid invitationId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var user = await db.Users.FindAsync(userId);
        if (user == null)
            return Unauthorized();

        if (invitationId == Guid.Empty || user.NetworkId.HasValue)
            return BadRequest();

        var invitation = await db.Invitations
            .Include(inv => inv.Establishment)
            .FirstOrDefaultAsync(inv => inv.Id == invitationId);
        if (invitation == null || invitation.TargetUserId != userId)
            return BadRequest();

        user.NetworkId = invitation.Establishment.NetworkId;
        invitation.Status = InvitationStatus.Accepted;

        var userEstablishment = new UserEstablishmentEntity()
        {
            UserId = user.Id,
            EstablishmentId = invitation.EstablishmentId,
            RoleId = invitation.RoleId,
        };

        db.UserEstablishments.Add(userEstablishment);
        await db.SaveChangesAsync();

        return Ok(new { invitation.Establishment.NetworkId });
    }

    [HttpDelete("{invitationId:Guid}")]
    public async Task<IActionResult> DeleteInvitation(Guid invitationId)
    {
        if (invitationId == Guid.Empty)
            return BadRequest();

        var invitation = await db.Invitations.FindAsync(invitationId);
        if (invitation == null)
            return BadRequest();

        db.Invitations.Remove(invitation);
        await db.SaveChangesAsync();

        return Ok();
    }
}
