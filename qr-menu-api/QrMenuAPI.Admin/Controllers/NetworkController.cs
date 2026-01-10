using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QrMenuAPI.Admin.Consts;
using QrMenuAPI.Admin.Models.Network;
using QrMenuAPI.Admin.Utils;
using QrMenuAPI.Core;
using QrMenuAPI.Core.Entities;

namespace QrMenuAPI.Admin.Controllers;

[Route("network")]
public class NetworkController(AppDbContext db) : BaseApiController
{
    [HttpPost("establishment")]
    public async Task<IActionResult> CreateEstablishment([FromBody] CreateEstablishmentRequest req)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        var user = await db.Users.FindAsync(userId);
        if (user == null)
            return NotFound(ErrorCodes.UserNotFound);

        if (await db.Establishments.AnyAsync(x => x.Name == req.Name))
            return Conflict(ErrorCodes.DuplicateEstablishment);

        var network = await GetOrCreateNetwork(user, req.Name);
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
        if (networkId <= 0)
            return BadRequest(ErrorCodes.InvalidRequest);

        var network = await db.Networks
            .Include(x => x.Establishments)
            .FirstOrDefaultAsync(x => x.Id == networkId);
        if (network == null)
            return NotFound(ErrorCodes.NetworkNotFound);

        var response = new NetworkResponse()
        {
            Id = network.Id,
            Name = network.Name,
            Establishments = network.Establishments.Select(x => new EstablishmentResponse()
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address
            })
        };

        return Success(response);
    }
}
