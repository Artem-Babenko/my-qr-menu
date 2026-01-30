using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QrMenuAPI.Admin.Consts;
using QrMenuAPI.Admin.Models.Tables;
using QrMenuAPI.Core;
using QrMenuAPI.Core.Entities;

namespace QrMenuAPI.Admin.Controllers;

[Route("tables")]
public class TablesController(AppDbContext db) : BaseApiController
{
    [HttpGet("by-establishment/{establishmentId:int}")]
    public async Task<IActionResult> ByEstablishment([FromRoute] int establishmentId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (establishmentId <= 0)
            return BadRequest(ErrorCodes.InvalidRequest);

        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return Unauthorized(ErrorCodes.UserNotFound);

        if (!user.NetworkId.HasValue)
            return NotFound(ErrorCodes.NetworkNotFound);

        var hasAccess = await db.Establishments
            .AsNoTracking()
            .AnyAsync(e => e.Id == establishmentId && e.NetworkId == user.NetworkId.Value);
        if (!hasAccess)
            return NotFound(ErrorCodes.EstablishmentNotFound);

        var tables = await db.Tables
            .AsNoTracking()
            .Where(t => t.EstablishmentId == establishmentId)
            .OrderBy(t => t.Number)
            .Select(t => new TableResponse
            {
                Id = t.Id,
                Number = t.Number,
                EstablishmentId = t.EstablishmentId
            })
            .ToListAsync();

        return Success(tables);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateTableRequest req)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (req == null || !req.IsValid())
            return BadRequest(ErrorCodes.InvalidRequest);

        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return Unauthorized(ErrorCodes.UserNotFound);

        if (!user.NetworkId.HasValue)
            return NotFound(ErrorCodes.NetworkNotFound);

        var establishment = await db.Establishments
            .FirstOrDefaultAsync(e => e.Id == req.EstablishmentId && e.NetworkId == user.NetworkId.Value);
        if (establishment == null)
            return NotFound(ErrorCodes.EstablishmentNotFound);

        var number = req.Number.Trim();
        var duplicate = await db.Tables.AnyAsync(t =>
            t.EstablishmentId == req.EstablishmentId &&
            t.Number == number);
        if (duplicate)
            return Conflict(ErrorCodes.DuplicateTableNumber);

        var table = new TableEntity
        {
            EstablishmentId = req.EstablishmentId,
            Number = number
        };

        db.Tables.Add(table);
        await db.SaveChangesAsync();

        return Success(new TableResponse
        {
            Id = table.Id,
            Number = table.Number,
            EstablishmentId = table.EstablishmentId
        });
    }

    [HttpPut("{tableId:int}")]
    public async Task<IActionResult> Update([FromRoute] int tableId, [FromBody] UpdateTableRequest req)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (tableId <= 0 || req == null || !req.IsValid())
            return BadRequest(ErrorCodes.InvalidRequest);

        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return Unauthorized(ErrorCodes.UserNotFound);

        if (!user.NetworkId.HasValue)
            return NotFound(ErrorCodes.NetworkNotFound);

        var table = await db.Tables
            .Include(t => t.Establishment)
            .FirstOrDefaultAsync(t =>
                t.Id == tableId &&
                t.Establishment.NetworkId == user.NetworkId.Value);
        if (table == null)
            return NotFound(ErrorCodes.TableNotFound);

        var number = req.Number.Trim();
        var duplicate = await db.Tables.AnyAsync(t =>
            t.EstablishmentId == table.EstablishmentId &&
            t.Id != tableId &&
            t.Number == number);
        if (duplicate)
            return Conflict(ErrorCodes.DuplicateTableNumber);

        table.Number = number;
        await db.SaveChangesAsync();

        return Success(new TableResponse
        {
            Id = table.Id,
            Number = table.Number,
            EstablishmentId = table.EstablishmentId
        });
    }

    [HttpDelete("{tableId:int}")]
    public async Task<IActionResult> Delete([FromRoute] int tableId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (tableId <= 0)
            return BadRequest(ErrorCodes.InvalidRequest);

        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return Unauthorized(ErrorCodes.UserNotFound);

        if (!user.NetworkId.HasValue)
            return NotFound(ErrorCodes.NetworkNotFound);

        var table = await db.Tables
            .Include(t => t.Establishment)
            .FirstOrDefaultAsync(t =>
                t.Id == tableId &&
                t.Establishment.NetworkId == user.NetworkId.Value);
        if (table == null)
            return NotFound(ErrorCodes.TableNotFound);

        db.Tables.Remove(table);
        await db.SaveChangesAsync();

        return Success();
    }
}

