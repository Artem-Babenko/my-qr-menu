using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QrMenuAPI.Client.Consts;
using QrMenuAPI.Client.Models;
using QrMenuAPI.Core;
using QrMenuAPI.Core.Entities;
using QrMenuAPI.Core.Enums;

namespace QrMenuAPI.Client.Controllers;

[Route("orders")]
public class OrdersController(AppDbContext db) : BaseController
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest req)
    {
        if (req == null || !req.IsValid())
            return BadRequest(ErrorCodes.InvalidRequest);

        var table = await db.Tables
            .AsNoTracking()
            .Include(t => t.Establishment)
            .FirstOrDefaultAsync(t => t.Id == req.TableId);

        if (table == null)
            return NotFound(ErrorCodes.TableNotFound);

        var establishmentId = table.EstablishmentId;
        var networkId = table.Establishment.NetworkId;

        var (items, totalSum, error) = await BuildOrderItems(networkId, establishmentId, req.Items);
        if (error != null)
            return error;

        var now = DateTime.UtcNow;
        var today = now.Date;

        var nextNumber = (await db.Orders
            .AsNoTracking()
            .Where(o =>
                o.EstablishmentId == establishmentId &&
                o.CreatedAt >= today &&
                o.CreatedAt < today.AddDays(1))
            .MaxAsync(o => (int?)o.OrderNumber) ?? 0) + 1;

        var order = new OrderEntity
        {
            OrderNumber = nextNumber,
            Status = OrderStatus.New,
            Source = OrderSource.QrMenu,
            TotalSum = totalSum,
            IsPaid = false,
            CustomerFullName = string.IsNullOrWhiteSpace(req.CustomerFullName)
                ? null
                : req.CustomerFullName.Trim(),
            Comment = string.IsNullOrWhiteSpace(req.Comment)
                ? null
                : req.Comment.Trim(),
            CreatedAt = now,
            ClosedAt = null,
            EstablishmentId = establishmentId,
            TableId = req.TableId,
            CreatedByUserId = null,
            Items = items,
        };

        db.Orders.Add(order);
        await db.SaveChangesAsync();

        return Success(new CreateOrderResponse
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
        });
    }

    [HttpGet("by-table/{tableId:int}")]
    public async Task<IActionResult> ByTable([FromRoute] int tableId)
    {
        if (tableId <= 0)
            return BadRequest(ErrorCodes.InvalidRequest);

        var tableExists = await db.Tables.AsNoTracking().AnyAsync(t => t.Id == tableId);
        if (!tableExists)
            return NotFound(ErrorCodes.TableNotFound);

        var since = DateTime.UtcNow.Date;

        var orders = await db.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .Where(o => o.TableId == tableId && o.CreatedAt >= since)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        var result = orders.Select(o => new OrderResponse
        {
            Id = o.Id,
            OrderNumber = o.OrderNumber,
            Status = o.Status,
            TotalSum = o.TotalSum,
            CustomerFullName = o.CustomerFullName,
            Comment = o.Comment,
            CreatedAt = o.CreatedAt,
            Items = o.Items
                .OrderBy(i => i.Id)
                .Select(i => new OrderItemResponse
                {
                    Name = i.ProductNameSnapshot,
                    Quantity = i.Quantity,
                    Price = i.PriceSnapshot,
                    LineTotal = i.LineTotal,
                })
                .ToList()
        }).ToList();

        return Success(result);
    }

    private async Task<(List<OrderItemEntity> items, decimal totalSum, IActionResult? error)> BuildOrderItems(
        int networkId,
        int establishmentId,
        List<Models.OrderItemRequest> requestItems)
    {
        if (requestItems == null || requestItems.Count == 0)
            return ([], 0, BadRequest(ErrorCodes.OrderItemsRequired));

        if (requestItems.Any(i => i == null || !i.IsValid()))
            return ([], 0, BadRequest(ErrorCodes.InvalidRequest));

        var productIds = requestItems.Select(i => i.ProductId).Distinct().ToList();

        var products = await (
                from p in db.Products.AsNoTracking()
                join c in db.Categories.AsNoTracking() on p.CategoryId equals c.Id
                join pp in db.Prices.AsNoTracking()
                    on new { ProductId = p.Id, EstablishmentId = establishmentId }
                    equals new { pp.ProductId, pp.EstablishmentId }
                where p.NetworkId == networkId
                      && productIds.Contains(p.Id)
                      && pp.IsActive
                select new
                {
                    p.Id,
                    p.Name,
                    CategoryName = c.Name,
                    Price = pp.Price,
                })
            .ToListAsync();

        if (products.Count != productIds.Count)
            return ([], 0, BadRequest(ErrorCodes.ProductNotAvailable));

        var byId = products.ToDictionary(x => x.Id, x => x);

        var items = new List<OrderItemEntity>(requestItems.Count);
        decimal total = 0;
        foreach (var req in requestItems)
        {
            var p = byId[req.ProductId];
            var line = p.Price * req.Quantity;
            total += line;

            items.Add(new OrderItemEntity
            {
                ProductId = req.ProductId,
                ProductNameSnapshot = p.Name,
                CategoryNameSnapshot = p.CategoryName,
                PriceSnapshot = p.Price,
                Quantity = req.Quantity,
                LineTotal = line,
            });
        }

        return (items, total, null);
    }
}
