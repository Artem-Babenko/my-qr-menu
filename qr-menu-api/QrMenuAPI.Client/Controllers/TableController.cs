using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QrMenuAPI.Client.Consts;
using QrMenuAPI.Client.Models;
using QrMenuAPI.Core;

namespace QrMenuAPI.Client.Controllers;

[Route("table")]
public class TableController(AppDbContext db) : BaseController
{
    [HttpGet("{tableId:int}")]
    public async Task<IActionResult> GetTable([FromRoute] int tableId)
    {
        if (tableId <= 0)
            return BadRequest(ErrorCodes.InvalidRequest);

        var table = await db.Tables
            .AsNoTracking()
            .Include(t => t.Establishment)
            .FirstOrDefaultAsync(t => t.Id == tableId);

        if (table == null)
            return NotFound(ErrorCodes.TableNotFound);

        return Success(new TableResponse
        {
            TableId = table.Id,
            TableNumber = table.Number,
            EstablishmentId = table.EstablishmentId,
            EstablishmentName = table.Establishment.Name,
            EstablishmentAddress = table.Establishment.Address,
        });
    }
}
