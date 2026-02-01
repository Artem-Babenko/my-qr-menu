using Microsoft.EntityFrameworkCore;
using QrMenuAPI.Core;
using QrMenuAPI.Core.Enums;

namespace QrMenuAPI.Admin.Utils;

public static class PermissionUtils
{
    public static Task<bool> HasEstablishmentPermission(
        AppDbContext db,
        int userId,
        int establishmentId,
        PermissionType permission)
    {
        return db.UserEstablishments
            .AsNoTracking()
            .Where(ue => ue.UserId == userId && ue.EstablishmentId == establishmentId)
            .SelectMany(ue => ue.Role.Permissions)
            .AnyAsync(p => p.PermissionType == permission);
    }
}
