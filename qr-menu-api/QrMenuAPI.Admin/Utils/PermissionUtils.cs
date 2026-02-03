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

    public static Task<bool> HasAnyEstablishmentPermission(
        AppDbContext db,
        int userId,
        int establishmentId,
        IEnumerable<PermissionType> permissions)
    {
        var perms = permissions.ToList();
        if (perms.Count == 0) return Task.FromResult(false);

        return db.UserEstablishments
            .AsNoTracking()
            .Where(ue => ue.UserId == userId && ue.EstablishmentId == establishmentId)
            .SelectMany(ue => ue.Role.Permissions)
            .AnyAsync(p => perms.Contains(p.PermissionType));
    }

    public static Task<bool> HasNetworkPermission(
        AppDbContext db,
        int userId,
        int networkId,
        PermissionType permission)
    {
        return db.UserEstablishments
            .AsNoTracking()
            .Where(ue => ue.UserId == userId && ue.Establishment.NetworkId == networkId)
            .SelectMany(ue => ue.Role.Permissions)
            .AnyAsync(p => p.PermissionType == permission);
    }

    public static Task<bool> HasAnyNetworkPermission(
        AppDbContext db,
        int userId,
        int networkId,
        IEnumerable<PermissionType> permissions)
    {
        var perms = permissions.ToList();
        if (perms.Count == 0) return Task.FromResult(false);

        return db.UserEstablishments
            .AsNoTracking()
            .Where(ue => ue.UserId == userId && ue.Establishment.NetworkId == networkId)
            .SelectMany(ue => ue.Role.Permissions)
            .AnyAsync(p => perms.Contains(p.PermissionType));
    }
}
