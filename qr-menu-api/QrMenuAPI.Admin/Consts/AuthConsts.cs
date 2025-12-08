using System.IdentityModel.Tokens.Jwt;

namespace QrMenuAPI.Admin.Consts;

public class AuthConsts
{
    public const string USER_ID_CLAIM = JwtRegisteredClaimNames.Sub;
    public const string SESSION_ID_CLAIM = JwtRegisteredClaimNames.Sid;
}
