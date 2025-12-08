using Microsoft.IdentityModel.Tokens;
using QrMenuAPI.Admin.Consts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QrMenuAPI.Admin.Utils;

public static class JwtGenerator
{
    public static string Generate(
        string jwtSecret,
        int userId,
        Guid sessionId,
        DateTime expiresAt)
    {
        if (string.IsNullOrEmpty(jwtSecret))
            throw new ArgumentNullException(nameof(jwtSecret));

        var claims = new[]
        {
            new Claim(AuthConsts.USER_ID_CLAIM, userId.ToString()),
            new Claim(AuthConsts.SESSION_ID_CLAIM, sessionId.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
