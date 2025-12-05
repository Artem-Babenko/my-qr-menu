using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QrMenuAPI.APP.Models.Auth;
using QrMenuAPI.Core;
using QrMenuAPI.Core.Consts;
using QrMenuAPI.Core.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QrMenuAPI.APP.Controllers;

[Route("auth")]
public class AuthController : BaseApiController
{
    readonly AppDbContext db;
    readonly IConfiguration config;

    readonly TimeSpan LIFE_TIME = TimeSpan.FromHours(3);

    public AuthController(AppDbContext db, IConfiguration config)
    {
        this.db = db;
        this.config = config;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (request == null || !request.IsValid())
            return BadRequest();

        var user = await db.Users
            .FirstOrDefaultAsync(x => x.Phone == request.Phone);
        if (user == null)
            return Unauthorized();

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            return Unauthorized();

        var nowDate = DateTime.UtcNow;

        var session = new UserSessionEntity()
        {
            UserId = user.Id,
            CreatedAt = nowDate,
            LastActivityAt = nowDate,
            ExpiresAt = nowDate.Add(LIFE_TIME),
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = HttpContext.Request.Headers.UserAgent.ToString(),
        };

        await db.UserSessions.AddAsync(session);
        await db.SaveChangesAsync();

        var token = GenerateJwtToken(session);

        return Ok(new { Token = token });
    }

    [AllowAnonymous]
    [HttpPost("reg")]
    public async Task<IActionResult> Registration([FromBody] RegistrationRequest request)
    {
        if (request == null || !request.IsValid())
            return BadRequest();

        var existingUser = await db.Users
            .FirstOrDefaultAsync(x => x.Phone == request.Phone);
        if (existingUser != null)
            return BadRequest();

        var user = new UserEntity()
        {
            Name = request.Name,
            Surname = request.Surname,
            Email = request.Email,
            Phone = request.Phone,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        await db.Users.AddAsync(user);
        await db.SaveChangesAsync();

        return Ok(new { UserId = user.Id });
    }

    private string GenerateJwtToken(UserSessionEntity session)
    {
        var jwtSecret = config["JwtSecret"]
            ?? throw new Exception("Jwt secret not found!");

        var claims = new[]
        {
            new Claim(AuthConsts.USER_ID_CLAIM, session.UserId.ToString()),
            new Claim(AuthConsts.SESSION_ID_CLAIM, session.Id.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: session.ExpiresAt,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
