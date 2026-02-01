using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QrMenuAPI.Admin.Consts;
using QrMenuAPI.Admin.Models.Products;
using QrMenuAPI.Core;
using QrMenuAPI.Core.Entities;

namespace QrMenuAPI.Admin.Controllers;

[Route("products")]
public class ProductsController(AppDbContext db) : BaseApiController
{
    [HttpGet("by-network/{networkId:int}")]
    public async Task<IActionResult> ByNetwork([FromRoute] int networkId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (networkId <= 0)
            return BadRequest(ErrorCodes.InvalidRequest);

        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return Unauthorized(ErrorCodes.UserNotFound);

        if (!user.NetworkId.HasValue || user.NetworkId.Value != networkId)
            return NotFound(ErrorCodes.NetworkNotFound);

        var products = await db.Products
            .AsNoTracking()
            .Where(p => p.NetworkId == networkId)
            .OrderBy(p => p.Name)
            .Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                NetworkId = p.NetworkId,
                CategoryId = p.CategoryId,
                Prices = db.Prices
                    .AsNoTracking()
                    .Where(pp => pp.ProductId == p.Id)
                    .Select(pp => new ProductPriceItemModel
                    {
                        EstablishmentId = pp.EstablishmentId,
                        Price = pp.Price,
                        IsActive = pp.IsActive
                    })
                    .ToList()
            })
            .ToListAsync();

        return Success(products);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest req)
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

        if (!user.NetworkId.HasValue || user.NetworkId.Value != req.NetworkId)
            return NotFound(ErrorCodes.NetworkNotFound);

        var categoryExists = await db.Categories
            .AsNoTracking()
            .AnyAsync(c => c.Id == req.CategoryId && c.NetworkId == req.NetworkId);
        if (!categoryExists)
            return NotFound(ErrorCodes.CategoryNotFound);

        var name = req.Name.Trim();
        if (await db.Products.AnyAsync(p => p.NetworkId == req.NetworkId && p.Name == name))
            return Conflict(ErrorCodes.DuplicateProductName);

        var establishments = await db.Establishments
            .AsNoTracking()
            .Where(e => e.NetworkId == req.NetworkId)
            .Select(e => new { e.Id })
            .ToListAsync();

        var priceByEstablishmentId = req.Prices
            .GroupBy(p => p.EstablishmentId)
            .ToDictionary(g => g.Key, g => g.First());

        var product = new ProductEntity
        {
            Name = name,
            Description = req.Description.Trim(),
            NetworkId = req.NetworkId,
            CategoryId = req.CategoryId
        };

        db.Products.Add(product);
        await db.SaveChangesAsync();

        var pricesToAdd = establishments.Select(e =>
        {
            var item = priceByEstablishmentId.GetValueOrDefault(e.Id);
            return new ProductPriceEntity
            {
                ProductId = product.Id,
                EstablishmentId = e.Id,
                Price = item?.Price ?? 0,
                IsActive = item?.IsActive ?? true
            };
        }).ToList();

        db.Prices.AddRange(pricesToAdd);
        await db.SaveChangesAsync();

        return Success(new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            NetworkId = product.NetworkId,
            CategoryId = product.CategoryId,
            Prices = pricesToAdd.Select(pp => new ProductPriceItemModel
            {
                EstablishmentId = pp.EstablishmentId,
                Price = pp.Price,
                IsActive = pp.IsActive
            }).ToList()
        });
    }

    [HttpPut("{productId:int}")]
    public async Task<IActionResult> Update([FromRoute] int productId, [FromBody] UpdateProductRequest req)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (productId <= 0 || req == null || !req.IsValid())
            return BadRequest(ErrorCodes.InvalidRequest);

        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return Unauthorized(ErrorCodes.UserNotFound);

        if (!user.NetworkId.HasValue)
            return NotFound(ErrorCodes.NetworkNotFound);

        var product = await db.Products
            .FirstOrDefaultAsync(p => p.Id == productId && p.NetworkId == user.NetworkId.Value);
        if (product == null)
            return NotFound(ErrorCodes.ProductNotFound);

        var categoryExists = await db.Categories
            .AsNoTracking()
            .AnyAsync(c => c.Id == req.CategoryId && c.NetworkId == user.NetworkId.Value);
        if (!categoryExists)
            return NotFound(ErrorCodes.CategoryNotFound);

        var name = req.Name.Trim();
        if (await db.Products.AnyAsync(p =>
            p.NetworkId == user.NetworkId.Value &&
            p.Name == name &&
            p.Id != productId))
            return Conflict(ErrorCodes.DuplicateProductName);

        product.Name = name;
        product.Description = req.Description.Trim();
        product.CategoryId = req.CategoryId;

        var establishments = await db.Establishments
            .AsNoTracking()
            .Where(e => e.NetworkId == user.NetworkId.Value)
            .Select(e => new { e.Id })
            .ToListAsync();

        var existingPrices = await db.Prices
            .Where(pp => pp.ProductId == productId)
            .ToListAsync();

        var existingByEstId = existingPrices.ToDictionary(pp => pp.EstablishmentId, pp => pp);

        var reqByEstId = req.Prices
            .GroupBy(p => p.EstablishmentId)
            .ToDictionary(g => g.Key, g => g.First());

        foreach (var est in establishments)
        {
            var incoming = reqByEstId.GetValueOrDefault(est.Id);
            var price = incoming?.Price ?? 0;
            var active = incoming?.IsActive ?? true;

            if (existingByEstId.TryGetValue(est.Id, out var pp))
            {
                pp.Price = price;
                pp.IsActive = active;
            }
            else
            {
                db.Prices.Add(new ProductPriceEntity
                {
                    ProductId = productId,
                    EstablishmentId = est.Id,
                    Price = price,
                    IsActive = active
                });
            }
        }

        await db.SaveChangesAsync();

        var prices = await db.Prices
            .AsNoTracking()
            .Where(pp => pp.ProductId == productId)
            .Select(pp => new ProductPriceItemModel
            {
                EstablishmentId = pp.EstablishmentId,
                Price = pp.Price,
                IsActive = pp.IsActive
            })
            .ToListAsync();

        return Success(new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            NetworkId = product.NetworkId,
            CategoryId = product.CategoryId,
            Prices = prices
        });
    }

    [HttpDelete("{productId:int}")]
    public async Task<IActionResult> Delete([FromRoute] int productId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (productId <= 0)
            return BadRequest(ErrorCodes.InvalidRequest);

        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return Unauthorized(ErrorCodes.UserNotFound);

        if (!user.NetworkId.HasValue)
            return NotFound(ErrorCodes.NetworkNotFound);

        var product = await db.Products
            .FirstOrDefaultAsync(p => p.Id == productId && p.NetworkId == user.NetworkId.Value);
        if (product == null)
            return NotFound(ErrorCodes.ProductNotFound);

        var prices = await db.Prices
            .Where(pp => pp.ProductId == productId)
            .ToListAsync();

        db.Prices.RemoveRange(prices);
        db.Products.Remove(product);
        await db.SaveChangesAsync();

        return Success();
    }
}

