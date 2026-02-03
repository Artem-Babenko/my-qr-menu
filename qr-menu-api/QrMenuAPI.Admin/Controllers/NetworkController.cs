using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QrMenuAPI.Admin.Consts;
using QrMenuAPI.Admin.Models.Network;
using QrMenuAPI.Admin.Utils;
using QrMenuAPI.Core;
using QrMenuAPI.Core.Entities;
using QrMenuAPI.Core.Enums;

namespace QrMenuAPI.Admin.Controllers;

[Route("network")]
public class NetworkController(AppDbContext db) : BaseApiController
{
    [HttpPut("{networkId:int}")]
    public async Task<IActionResult> UpdateNetwork(
        [FromRoute] int networkId,
        [FromBody] UpdateNetworkRequest req)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (networkId <= 0 || req == null || !req.IsValid())
            return BadRequest(ErrorCodes.InvalidRequest);

        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return Unauthorized(ErrorCodes.UserNotFound);

        if (!user.NetworkId.HasValue || user.NetworkId.Value != networkId)
            return NotFound(ErrorCodes.NetworkNotFound);

        var canEdit = await PermissionUtils.HasAnyNetworkPermission(
            db,
            userId,
            networkId,
            [PermissionType.NetworkEdit]);
        if (!canEdit)
            return Forbidden(ErrorCodes.PermissionDenied);

        var network = await db.Networks.FirstOrDefaultAsync(n => n.Id == networkId);
        if (network == null)
            return NotFound(ErrorCodes.NetworkNotFound);

        var newName = req.Name.Trim();
        if (await db.Networks.AnyAsync(n => n.Name == newName && n.Id != networkId))
            return Conflict(ErrorCodes.DuplicateNetwork);

        network.Name = newName;
        await db.SaveChangesAsync();

        return Success();
    }

    [HttpPost("establishment")]
    public async Task<IActionResult> CreateEstablishment([FromBody] CreateEstablishmentRequest req)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        var user = await db.Users.FindAsync(userId);
        if (user == null)
            return NotFound(ErrorCodes.UserNotFound);

        if (req == null || !req.IsValid())
            return BadRequest(ErrorCodes.InvalidRequest);

        if (await db.Establishments.AnyAsync(x => x.Name == req.Name))
            return Conflict(ErrorCodes.DuplicateEstablishment);

        var isCreatingNetwork = user.NetworkId == null;
        var requestedNetworkName = (req.NetworkName ?? req.Name).Trim();
        if (isCreatingNetwork)
        {
            if (await db.Networks.AnyAsync(n => n.Name == requestedNetworkName))
                return Conflict(ErrorCodes.DuplicateNetwork);
        }
        else
        {
            var canCreate = await PermissionUtils.HasAnyNetworkPermission(
                db,
                userId,
                user.NetworkId.Value,
                [PermissionType.EstablishmentsCreate]);
            if (!canCreate)
                return Forbidden(ErrorCodes.PermissionDenied);
        }

        var network = await GetOrCreateNetwork(user, requestedNetworkName);

        if (!string.IsNullOrWhiteSpace(req.NetworkName) && user.NetworkId != null)
        {
            var newName = req.NetworkName.Trim();
            if (await db.Networks.AnyAsync(n => n.Name == newName && n.Id != network.Id))
                return Conflict(ErrorCodes.DuplicateNetwork);
            network.Name = newName;
        }

        var establishment = CreateEstablishment(req.Name, req.Address, network, userId);

        await db.SaveChangesAsync();

        return Success(new
        {
            NetworkId = network.Id,
            EstablishmentId = establishment.Id
        });
    }

    private async Task<NetworkEntity> GetOrCreateNetwork(
        UserEntity user,
        string networkName)
    {
        if (user.NetworkId != null)
        {
            return await db.Networks
                .Include(n => n.Roles)
                .FirstAsync(n => n.Id == user.NetworkId);
        }

        var network = new NetworkEntity
        {
            Name = networkName,
            CreatedAt = DateTime.UtcNow,
            Roles = DefaultRoles.Get()
        };

        db.Networks.Add(network);
        user.Network = network;
        db.Users.Update(user);

        return network;
    }

    private EstablishmentEntity CreateEstablishment(
        string name,
        string address,
        NetworkEntity network,
        int userId)
    {
        var establishment = new EstablishmentEntity
        {
            Name = name,
            Address = address,
            Network = network,
            CreatedAt = DateTime.UtcNow,
            UserEstablishment =
            [
                new()
                {
                    UserId = userId,
                    Role = network.Roles.First(r => r.Name == DefaultRoles.Owner)
                }
            ]
        };

        db.Establishments.Add(establishment);
        return establishment;
    }

    [HttpGet("{networkId:int}")]
    public async Task<IActionResult> GetNetwork([FromRoute] int networkId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (networkId <= 0)
            return BadRequest(ErrorCodes.InvalidRequest);

        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return Unauthorized(ErrorCodes.UserNotFound);

        if (!user.NetworkId.HasValue || user.NetworkId.Value != networkId)
            return NotFound(ErrorCodes.NetworkNotFound);

        var network = await db.Networks
            .Include(x => x.Establishments)
            .FirstOrDefaultAsync(x => x.Id == networkId);
        if (network == null)
            return NotFound(ErrorCodes.NetworkNotFound);

        var establishmentIds = network.Establishments
            .Select(e => e.Id)
            .ToList();

        var usersCountByEstablishmentId = await db.UserEstablishments
            .AsNoTracking()
            .Where(ue => establishmentIds.Contains(ue.EstablishmentId))
            .GroupBy(ue => ue.EstablishmentId)
            .Select(g => new { EstablishmentId = g.Key, UsersCount = g.Count() })
            .ToDictionaryAsync(x => x.EstablishmentId, x => x.UsersCount);

        var response = new NetworkResponse()
        {
            Id = network.Id,
            Name = network.Name,
            Establishments = network.Establishments.Select(x => new EstablishmentResponse()
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address,
                UsersCount = usersCountByEstablishmentId.GetValueOrDefault(x.Id, 0)
            })
        };

        return Success(response);
    }
}
