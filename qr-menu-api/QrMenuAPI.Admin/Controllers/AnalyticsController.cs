using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QrMenuAPI.Admin.Consts;
using QrMenuAPI.Admin.Models.Analytics;
using QrMenuAPI.Core;
using QrMenuAPI.Core.Enums;

namespace QrMenuAPI.Admin.Controllers;

[Route("analytics")]
public class AnalyticsController(AppDbContext db) : BaseApiController
{
    [HttpGet("network/{networkId:int}")]
    public async Task<IActionResult> GetOverview(
        [FromRoute] int networkId,
        [FromQuery] DateTime dateFrom,
        [FromQuery] DateTime dateTo)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (networkId <= 0)
            return BadRequest(ErrorCodes.InvalidRequest);

        var fromUtc = dateFrom.ToUniversalTime();
        var toUtc = dateTo.ToUniversalTime();
        if (fromUtc == default || toUtc == default || fromUtc > toUtc)
            return BadRequest(ErrorCodes.InvalidRequest);

        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return Unauthorized(ErrorCodes.UserNotFound);

        if (!user.NetworkId.HasValue || user.NetworkId.Value != networkId)
            return NotFound(ErrorCodes.NetworkNotFound);

        var allowedEstablishmentIds = await db.UserEstablishments
            .AsNoTracking()
            .Where(ue =>
                ue.UserId == userId &&
                ue.Establishment.NetworkId == networkId &&
                ue.Role.Permissions.Any(p => p.PermissionType == PermissionType.AnalyticsView))
            .Select(ue => ue.EstablishmentId)
            .Distinct()
            .ToListAsync();

        if (allowedEstablishmentIds.Count == 0)
            return Forbidden(ErrorCodes.PermissionDenied);

        var completedOrdersQuery = db.Orders
            .AsNoTracking()
            .Where(o =>
                o.Establishment.NetworkId == networkId &&
                allowedEstablishmentIds.Contains(o.EstablishmentId) &&
                o.Status == OrderStatus.Completed &&
                o.ClosedAt.HasValue &&
                o.ClosedAt.Value >= fromUtc &&
                o.ClosedAt.Value <= toUtc);

        var orderRows = await completedOrdersQuery
            .Select(o => new
            {
                o.Id,
                o.CreatedAt,
                ClosedAt = o.ClosedAt!.Value,
                o.TotalSum,
            })
            .ToListAsync();

        var totalOrders = orderRows.Count;
        var totalSales = orderRows.Sum(x => x.TotalSum);
        var averageCheck = totalOrders > 0 ? totalSales / totalOrders : 0m;
        var averageTimeMinutes = totalOrders > 0
            ? orderRows.Average(x => Math.Max(0, (x.ClosedAt - x.CreatedAt).TotalMinutes))
            : 0d;

        var salesByHourValues = Enumerable.Range(0, 24).ToDictionary(hour => hour, _ => 0m);
        var ordersByHourValues = Enumerable.Range(0, 24).ToDictionary(hour => hour, _ => 0m);
        foreach (var row in orderRows)
        {
            var hour = row.ClosedAt.Hour;
            salesByHourValues[hour] += row.TotalSum;
            ordersByHourValues[hour] += 1;
        }

        var startDayUtc = fromUtc.Date;
        var endDayUtc = toUtc.Date;
        var dayCount = Math.Max(1, (endDayUtc - startDayUtc).Days + 1);
        var dayKeys = Enumerable.Range(0, dayCount)
            .Select(offset => startDayUtc.AddDays(offset))
            .ToList();

        var salesByDayValues = dayKeys.ToDictionary(day => day, _ => 0m);
        var ordersByDayValues = dayKeys.ToDictionary(day => day, _ => 0m);
        foreach (var row in orderRows)
        {
            var day = row.ClosedAt.Date;
            if (salesByDayValues.ContainsKey(day))
            {
                salesByDayValues[day] += row.TotalSum;
                ordersByDayValues[day] += 1;
            }
        }

        var popularDishes = await db.OrderItems
            .AsNoTracking()
            .Where(oi => completedOrdersQuery.Select(o => o.Id).Contains(oi.OrderId))
            .GroupBy(oi => oi.ProductNameSnapshot)
            .Select(g => new PopularDishResponse
            {
                Name = g.Key,
                OrdersCount = g.Select(x => x.OrderId).Distinct().Count(),
                TotalSum = g.Sum(x => x.LineTotal),
            })
            .OrderByDescending(x => x.TotalSum)
            .Take(5)
            .ToListAsync();

        var response = new AnalyticsOverviewResponse
        {
            DateFrom = fromUtc,
            DateTo = toUtc,
            TotalSales = totalSales,
            TotalOrders = totalOrders,
            AverageCheck = averageCheck,
            AverageTimeMinutes = averageTimeMinutes,
            SalesByHours = Enumerable.Range(0, 24)
                .Select(hour => new AnalyticsPointResponse
                {
                    Key = hour.ToString(),
                    Label = $"{hour:00}:00",
                    Value = salesByHourValues[hour],
                })
                .ToList(),
            OrdersByHours = Enumerable.Range(0, 24)
                .Select(hour => new AnalyticsPointResponse
                {
                    Key = hour.ToString(),
                    Label = $"{hour:00}:00",
                    Value = ordersByHourValues[hour],
                })
                .ToList(),
            SalesByDays = dayKeys
                .Select(day => new AnalyticsPointResponse
                {
                    Key = day.ToString("yyyy-MM-dd"),
                    Label = day.ToString("dd.MM"),
                    Value = salesByDayValues[day],
                })
                .ToList(),
            OrdersByDays = dayKeys
                .Select(day => new AnalyticsPointResponse
                {
                    Key = day.ToString("yyyy-MM-dd"),
                    Label = day.ToString("dd.MM"),
                    Value = ordersByDayValues[day],
                })
                .ToList(),
            PopularDishes = popularDishes,
        };

        return Success(response);
    }
}
