using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QrMenuAPI.Admin.Consts;
using QrMenuAPI.Admin.Models.Auth;
using QrMenuAPI.Admin.Models.Options;
using QrMenuAPI.Admin.Services.Invitations;
using QrMenuAPI.Admin.Utils;
using QrMenuAPI.Core;
using QrMenuAPI.Core.Entities;

namespace QrMenuAPI.Admin.Controllers;

[Route("auth")]
public class AuthController(
    AppDbContext db,
    IInvitationService invitationService,
    IOptions<AppSettings> options) : BaseApiController
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (request == null || !request.IsValid())
            return BadRequest(ErrorCodes.InvalidRequest);

        var user = await db.Users.FirstOrDefaultAsync(x => x.Phone == request.Phone);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            return Unauthorized(ErrorCodes.InvalidCredentials);

        var nowDate = DateTime.UtcNow;

        var session = new UserSessionEntity()
        {
            UserId = user.Id,
            CreatedAt = nowDate,
            LastActivityAt = nowDate,
            ExpiresAt = nowDate.Add(options.Value.JwtLifeTime),
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = HttpContext.Request.Headers.UserAgent.ToString(),
        };

        await db.UserSessions.AddAsync(session);
        await db.SaveChangesAsync();

        var token = JwtGenerator.Generate(
            options.Value.JwtSecret,
            session.UserId,
            session.Id,
            session.ExpiresAt);

        return Success(new { Token = token });
    }

    [AllowAnonymous]
    [HttpPost("reg")]
    public async Task<IActionResult> Registration([FromBody] RegistrationRequest request)
    {
        if (request == null || !request.IsValid())
            return BadRequest(ErrorCodes.InvalidRequest);

        if (await HasUserWithPhone(request.Phone))
            return Conflict(ErrorCodes.DuplicatePhone);

        var user = await RegistrateUser(request);

        return Success(new { UserId = user.Id });
    }

    [AllowAnonymous]
    [HttpPost("reg-with-invite")]
    public async Task<IActionResult> RegistationWithInvite([FromBody] RegistrationWithInvitation request)
    {
        if (request == null || !request.IsValid())
            return BadRequest(ErrorCodes.InvalidRequest);

        if (await HasUserWithPhone(request.Phone))
            return Conflict(ErrorCodes.DuplicatePhone);

        var invitation = await db.Invitations.FindAsync(request.InvitationId);
        if (invitation == null || invitation.ExpiredAt <= DateTime.UtcNow)
            return NotFound(ErrorCodes.InvitationNotFound);

        using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            var user = await RegistrateUser(request);
            await invitationService.AcceptInvitation(invitation, user);
            await transaction.CommitAsync();

            return Success(new
            {
                UserId = user.Id,
                invitation.Establishment.NetworkId
            });
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task<UserEntity> RegistrateUser(RegistrationRequest model)
    {
        var user = new UserEntity()
        {
            Name = model.Name,
            Surname = model.Surname,
            Email = model.Email,
            Phone = model.Phone,
            Password = BCrypt.Net.BCrypt.HashPassword(model.Password)
        };

        await db.Users.AddAsync(user);
        await db.SaveChangesAsync();

        return user;
    }

    private Task<bool> HasUserWithPhone(string phone)
    {
        return db.Users.AnyAsync(x => x.Phone == phone);
    }
}
