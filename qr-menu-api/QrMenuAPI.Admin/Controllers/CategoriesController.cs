using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QrMenuAPI.Admin.Consts;
using QrMenuAPI.Admin.Models.Categories;
using QrMenuAPI.Core;
using QrMenuAPI.Core.Entities;

namespace QrMenuAPI.Admin.Controllers;

[Route("categories")]
public class CategoriesController(AppDbContext db) : BaseApiController
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

        var categories = await db.Categories
            .AsNoTracking()
            .Where(c => c.NetworkId == networkId)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                SortOrder = c.SortOrder,
                IsActive = c.IsActive,
                NetworkId = c.NetworkId
            })
            .ToListAsync();

        return Success(categories);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest req)
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

        var name = req.Name.Trim();
        if (await db.Categories.AnyAsync(c => c.NetworkId == req.NetworkId && c.Name == name))
            return Conflict(ErrorCodes.DuplicateCategoryName);

        var category = new CategoryEntity
        {
            Name = name,
            Description = req.Description?.Trim(),
            SortOrder = req.SortOrder,
            IsActive = req.IsActive,
            NetworkId = req.NetworkId
        };

        db.Categories.Add(category);
        await db.SaveChangesAsync();

        return Success(new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            SortOrder = category.SortOrder,
            IsActive = category.IsActive,
            NetworkId = category.NetworkId
        });
    }

    [HttpPut("{categoryId:int}")]
    public async Task<IActionResult> Update([FromRoute] int categoryId, [FromBody] UpdateCategoryRequest req)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (categoryId <= 0 || req == null || !req.IsValid())
            return BadRequest(ErrorCodes.InvalidRequest);

        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return Unauthorized(ErrorCodes.UserNotFound);

        if (!user.NetworkId.HasValue)
            return NotFound(ErrorCodes.NetworkNotFound);

        var category = await db.Categories
            .FirstOrDefaultAsync(c => c.Id == categoryId && c.NetworkId == user.NetworkId.Value);
        if (category == null)
            return NotFound(ErrorCodes.CategoryNotFound);

        var name = req.Name.Trim();
        if (await db.Categories.AnyAsync(c =>
            c.NetworkId == user.NetworkId.Value &&
            c.Name == name &&
            c.Id != categoryId))
            return Conflict(ErrorCodes.DuplicateCategoryName);

        category.Name = name;
        category.Description = req.Description?.Trim();
        category.SortOrder = req.SortOrder;
        category.IsActive = req.IsActive;

        await db.SaveChangesAsync();

        return Success(new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            SortOrder = category.SortOrder,
            IsActive = category.IsActive,
            NetworkId = category.NetworkId
        });
    }

    [HttpDelete("{categoryId:int}")]
    public async Task<IActionResult> Delete([FromRoute] int categoryId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (categoryId <= 0)
            return BadRequest(ErrorCodes.InvalidRequest);

        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return Unauthorized(ErrorCodes.UserNotFound);

        if (!user.NetworkId.HasValue)
            return NotFound(ErrorCodes.NetworkNotFound);

        var category = await db.Categories
            .FirstOrDefaultAsync(c => c.Id == categoryId && c.NetworkId == user.NetworkId.Value);
        if (category == null)
            return NotFound(ErrorCodes.CategoryNotFound);

        var hasProducts = await db.Products.AnyAsync(p => p.CategoryId == categoryId);
        if (hasProducts)
            return Conflict(ErrorCodes.CategoryDeleteForbidden);

        db.Categories.Remove(category);
        await db.SaveChangesAsync();
        return Success();
    }
}

