using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QrMenuAPI.Client.Consts;
using QrMenuAPI.Client.Models;
using QrMenuAPI.Core;

namespace QrMenuAPI.Client.Controllers;

[Route("menu")]
public class MenuController(AppDbContext db) : BaseController
{
    [HttpGet("{establishmentId:int}")]
    public async Task<IActionResult> GetMenu([FromRoute] int establishmentId)
    {
        if (establishmentId <= 0)
            return BadRequest(ErrorCodes.InvalidRequest);

        var establishment = await db.Establishments
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == establishmentId);

        if (establishment == null)
            return NotFound(ErrorCodes.EstablishmentNotFound);

        var networkId = establishment.NetworkId;

        var categories = await db.Categories
            .AsNoTracking()
            .Where(c => c.NetworkId == networkId && c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ToListAsync();

        var productsWithPrices = await (
                from p in db.Products.AsNoTracking()
                join pp in db.Prices.AsNoTracking()
                    on new { ProductId = p.Id, EstablishmentId = establishmentId }
                    equals new { pp.ProductId, pp.EstablishmentId }
                where p.NetworkId == networkId && pp.IsActive
                select new
                {
                    p.Id,
                    p.Name,
                    p.Description,
                    p.CategoryId,
                    pp.Price,
                })
            .ToListAsync();

        var productsByCategory = productsWithPrices
            .GroupBy(p => p.CategoryId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var result = categories
            .Where(c => productsByCategory.ContainsKey(c.Id))
            .Select(c => new MenuCategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                SortOrder = c.SortOrder,
                Products = productsByCategory[c.Id]
                    .Select(p => new MenuProductResponse
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                    })
                    .ToList()
            })
            .ToList();

        return Success(result);
    }
}
