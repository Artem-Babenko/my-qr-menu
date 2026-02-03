using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QrMenuAPI.Admin.Consts;
using QrMenuAPI.Admin.Models.Orders;
using QrMenuAPI.Admin.Utils;
using QrMenuAPI.Core;
using QrMenuAPI.Core.Entities;
using QrMenuAPI.Core.Enums;

namespace QrMenuAPI.Admin.Controllers;

[Route("orders")]
public class OrdersController(AppDbContext db) : BaseApiController
{
    private static readonly PermissionType[] OrdersAnyAccessPermissions =
    [
        PermissionType.OrdersView,
        PermissionType.OrdersCreate,
        PermissionType.OrdersEdit,
        PermissionType.OrdersTakeInWork,
        PermissionType.OrdersSendToKitchen,
        PermissionType.OrdersStartCooking,
        PermissionType.OrdersMarkReady,
        PermissionType.OrdersReturn,
        PermissionType.OrdersComplete,
        PermissionType.OrdersCancel,
        PermissionType.OrdersDelete,
    ];

    [HttpGet("by-network/{networkId:int}")]
    public async Task<IActionResult> ByNetwork([FromRoute] int networkId)
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

        var allowedEstablishmentIds = await db.UserEstablishments
            .AsNoTracking()
            .Where(ue => ue.UserId == userId && ue.Establishment.NetworkId == networkId)
            .Where(ue => ue.Role.Permissions.Any(p => OrdersAnyAccessPermissions.Contains(p.PermissionType)))
            .Select(ue => ue.EstablishmentId)
            .Distinct()
            .ToListAsync();

        if (allowedEstablishmentIds.Count == 0)
            return Forbidden(ErrorCodes.PermissionDenied);

        var orders = await db.Orders
            .AsNoTracking()
            .Include(o => o.Establishment)
            .Include(o => o.Table)
            .Include(o => o.Items)
            .Include(o => o.Staff)
                .ThenInclude(s => s.User)
            .Where(o => o.Establishment.NetworkId == networkId && allowedEstablishmentIds.Contains(o.EstablishmentId))
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        var models = orders.Select(o =>
        {
            var waiter = o.Staff.FirstOrDefault(s => s.Role == OrderStaffRole.Waiter);
            return new OrderListItemResponse
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                ClosedAt = o.ClosedAt,
                EstablishmentId = o.EstablishmentId,
                EstablishmentName = o.Establishment.Name,
                TableId = o.TableId,
                TableNumber = o.Table?.Number,
                WaiterId = waiter?.UserId,
                WaiterFullName = waiter == null ? null : $"{waiter.User.Name} {waiter.User.Surname}",
                Items = o.Items
                    .OrderBy(i => i.Id)
                    .Select(i => new OrderItemShortResponse
                    {
                        Name = i.ProductNameSnapshot,
                        Quantity = i.Quantity
                    })
                    .ToList()
            };
        }).ToList();

        return Success(models);
    }

    [HttpGet("{orderId:int}")]
    public async Task<IActionResult> GetById([FromRoute] int orderId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (orderId <= 0)
            return BadRequest(ErrorCodes.InvalidRequest);

        var (networkId, userErr) = await GetUserNetworkId(userId);
        if (userErr != null)
            return userErr;

        var order = await db.Orders
            .AsNoTracking()
            .Include(o => o.Establishment)
            .Include(o => o.Table)
            .Include(o => o.CreatedByUser)
            .Include(o => o.Items)
            .Include(o => o.Staff).ThenInclude(s => s.User)
            .Include(o => o.StatusHistory).ThenInclude(h => h.ChangedByUser)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.Establishment.NetworkId == networkId);

        if (order == null)
            return NotFound(ErrorCodes.OrderNotFound);

        var canView = await PermissionUtils.HasAnyEstablishmentPermission(
            db,
            userId,
            order.EstablishmentId,
            OrdersAnyAccessPermissions);
        if (!canView)
            return Forbidden(ErrorCodes.PermissionDenied);

        var model = new OrderDetailsResponse
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status,
            Source = order.Source,
            TotalSum = order.TotalSum,
            IsPaid = order.IsPaid,
            CustomerFullName = order.CustomerFullName,
            Comment = order.Comment,
            CreatedAt = order.CreatedAt,
            ClosedAt = order.ClosedAt,
            EstablishmentId = order.EstablishmentId,
            EstablishmentName = order.Establishment.Name,
            TableId = order.TableId,
            TableNumber = order.Table?.Number,
            CreatedByUserId = order.CreatedByUserId,
            CreatedByUserFullName = order.CreatedByUser == null ? null : $"{order.CreatedByUser.Name} {order.CreatedByUser.Surname}",
            Items = order.Items
                .OrderBy(i => i.Id)
                .Select(i => new OrderItemDetailsResponse
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.ProductNameSnapshot,
                    CategoryName = i.CategoryNameSnapshot,
                    Price = i.PriceSnapshot,
                    Quantity = i.Quantity,
                    LineTotal = i.LineTotal
                })
                .ToList(),
            Staff = order.Staff
                .OrderBy(s => s.AssignedAt)
                .Select(s => new OrderStaffResponse
                {
                    UserId = s.UserId,
                    FullName = $"{s.User.Name} {s.User.Surname}",
                    Role = s.Role,
                    AssignedAt = s.AssignedAt
                })
                .ToList(),
            StatusHistory = order.StatusHistory
                .OrderByDescending(h => h.ChangedAt)
                .Select(h => new OrderStatusHistoryResponse
                {
                    Id = h.Id,
                    FromStatus = h.FromStatus,
                    ToStatus = h.ToStatus,
                    ChangedAt = h.ChangedAt,
                    ChangedByUserId = h.ChangedByUserId,
                    ChangedByUserFullName = $"{h.ChangedByUser.Name} {h.ChangedByUser.Surname}"
                })
                .ToList()
        };

        return Success(model);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest req)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (req == null || !req.IsValid())
            return BadRequest(ErrorCodes.InvalidRequest);

        var (networkId, userErr) = await GetUserNetworkId(userId);
        if (userErr != null)
            return userErr;

        var establishment = await db.Establishments
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == req.EstablishmentId && e.NetworkId == networkId);
        if (establishment == null)
            return NotFound(ErrorCodes.EstablishmentNotFound);

        var canCreate = await HasEstablishmentPermission(
            userId,
            req.EstablishmentId,
            PermissionType.OrdersCreate);
        if (!canCreate)
            return Forbidden(ErrorCodes.PermissionDenied);

        if (req.Items == null || req.Items.Count == 0)
            return BadRequest(ErrorCodes.OrderItemsRequired);

        var table = req.TableId.HasValue
            ? await db.Tables
                .AsNoTracking()
                .FirstOrDefaultAsync(t =>
                    t.Id == req.TableId.Value &&
                    t.EstablishmentId == req.EstablishmentId)
            : null;
        if (req.TableId.HasValue && table == null)
            return NotFound(ErrorCodes.TableNotFound);

        if (req.WaiterId.HasValue)
        {
            var waiterHasAccess = await db.UserEstablishments
                .AsNoTracking()
                .AnyAsync(ue =>
                    ue.UserId == req.WaiterId.Value &&
                    ue.EstablishmentId == req.EstablishmentId);
            if (!waiterHasAccess)
                return BadRequest(ErrorCodes.InvalidRequest);
        }

        var now = DateTime.UtcNow;
        var today = now.Date;

        var nextNumber = (await db.Orders
            .AsNoTracking()
            .Where(o =>
                o.EstablishmentId == req.EstablishmentId &&
                o.CreatedAt >= today &&
                o.CreatedAt < today.AddDays(1))
            .MaxAsync(o => (int?)o.OrderNumber) ?? 0) + 1;

        var (items, totalSum, itemsErr) = await BuildOrderItems(
            networkId,
            req.EstablishmentId,
            req.Items);
        if (itemsErr != null)
            return itemsErr;

        var status = req.WaiterId.HasValue ? OrderStatus.Accepted : OrderStatus.New;

        var order = new OrderEntity
        {
            OrderNumber = nextNumber,
            Status = status,
            Source = OrderSource.AdminPanel,
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
            EstablishmentId = req.EstablishmentId,
            TableId = req.TableId,
            CreatedByUserId = userId,
            Items = items
        };

        if (req.WaiterId.HasValue)
        {
            order.Staff.Add(new OrderStaffEntity
            {
                Role = OrderStaffRole.Waiter,
                AssignedAt = now,
                UserId = req.WaiterId.Value
            });
        }

        db.Orders.Add(order);
        await db.SaveChangesAsync();

        if (status != OrderStatus.New)
        {
            db.OrderStatusHistories.Add(new OrderStatusHistoryEntity
            {
                OrderId = order.Id,
                FromStatus = OrderStatus.New,
                ToStatus = status,
                ChangedAt = now,
                ChangedByUserId = userId
            });
            await db.SaveChangesAsync();
        }

        return Success(new { orderId = order.Id });
    }

    [HttpPut("{orderId:int}")]
    public async Task<IActionResult> Update(
        [FromRoute] int orderId,
        [FromBody] UpdateOrderRequest req)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (orderId <= 0 || req == null || !req.IsValid())
            return BadRequest(ErrorCodes.InvalidRequest);

        var (networkId, userErr) = await GetUserNetworkId(userId);
        if (userErr != null)
            return userErr;

        var order = await db.Orders
            .Include(o => o.Establishment)
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o =>
                o.Id == orderId &&
                o.Establishment.NetworkId == networkId);
        if (order == null)
            return NotFound(ErrorCodes.OrderNotFound);

        var canEdit = await HasEstablishmentPermission(
            userId,
            order.EstablishmentId,
            PermissionType.OrdersEdit);
        if (!canEdit)
            return Forbidden(ErrorCodes.PermissionDenied);

        if (order.Status is OrderStatus.Completed or OrderStatus.Cancelled)
            return Conflict(ErrorCodes.OrderInvalidStatus);

        if (req.TableId.HasValue)
        {
            var table = await db.Tables
                .AsNoTracking()
                .FirstOrDefaultAsync(t =>
                    t.Id == req.TableId.Value &&
                    t.EstablishmentId == order.EstablishmentId);
            if (table == null)
                return NotFound(ErrorCodes.TableNotFound);
        }

        var (items, totalSum, itemsErr) = await BuildOrderItems(
            networkId,
            order.EstablishmentId,
            req.Items);
        if (itemsErr != null)
            return itemsErr;

        order.TableId = req.TableId;
        order.CustomerFullName = string.IsNullOrWhiteSpace(req.CustomerFullName)
            ? null
            : req.CustomerFullName.Trim();
        order.Comment = string.IsNullOrWhiteSpace(req.Comment)
            ? null
            : req.Comment.Trim();
        order.TotalSum = totalSum;

        db.OrderItems.RemoveRange(order.Items);
        order.Items.Clear();
        foreach (var it in items)
            order.Items.Add(it);

        await db.SaveChangesAsync();
        return Success();
    }

    [HttpPost("{orderId:int}/take-in-work")]
    public async Task<IActionResult> TakeInWork([FromRoute] int orderId) =>
        await ChangeStatusWithWaiterAssignment(
            orderId,
            OrderStatus.Accepted,
            PermissionType.OrdersTakeInWork,
            onlyIfCurrent: OrderStatus.New);

    [HttpPost("{orderId:int}/send-to-kitchen")]
    public async Task<IActionResult> SendToKitchen([FromRoute] int orderId) =>
        await ChangeStatusSimple(
            orderId,
            OrderStatus.InKitchen,
            PermissionType.OrdersSendToKitchen,
            onlyIfCurrent: OrderStatus.Accepted);

    [HttpPost("{orderId:int}/start-cooking")]
    public async Task<IActionResult> StartCooking([FromRoute] int orderId) =>
        await ChangeStatusWithCookAssignment(
            orderId,
            OrderStatus.Cooking,
            PermissionType.OrdersStartCooking,
            onlyIfCurrent: OrderStatus.InKitchen);

    [HttpPost("{orderId:int}/mark-ready")]
    public async Task<IActionResult> MarkReady([FromRoute] int orderId) =>
        await ChangeStatusSimple(
            orderId,
            OrderStatus.Ready,
            PermissionType.OrdersMarkReady,
            onlyIfCurrent: OrderStatus.Cooking);

    [HttpPost("{orderId:int}/complete")]
    public async Task<IActionResult> Complete([FromRoute] int orderId) =>
        await ChangeStatusSimple(
            orderId,
            OrderStatus.Completed,
            PermissionType.OrdersComplete,
            onlyIfCurrent: OrderStatus.Ready,
            setClosedAt: true);

    [HttpPost("{orderId:int}/cancel")]
    public async Task<IActionResult> Cancel([FromRoute] int orderId) =>
        await ChangeStatusSimple(
            orderId,
            OrderStatus.Cancelled,
            PermissionType.OrdersCancel,
            setClosedAt: true,
            allowFromStatuses: null);

    [HttpPost("{orderId:int}/return")]
    public async Task<IActionResult> Return([FromRoute] int orderId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (orderId <= 0)
            return BadRequest(ErrorCodes.InvalidRequest);

        var (networkId, userErr) = await GetUserNetworkId(userId);
        if (userErr != null)
            return userErr;

        var order = await db.Orders
            .Include(o => o.Establishment)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.Establishment.NetworkId == networkId);
        if (order == null)
            return NotFound(ErrorCodes.OrderNotFound);

        var canReturn = await HasEstablishmentPermission(
            userId,
            order.EstablishmentId,
            PermissionType.OrdersReturn);
        if (!canReturn)
            return Forbidden(ErrorCodes.PermissionDenied);

        var target = order.Status switch
        {
            OrderStatus.InKitchen => OrderStatus.Accepted,
            OrderStatus.Cooking => OrderStatus.InKitchen,
            OrderStatus.Ready => OrderStatus.InKitchen,
            _ => (OrderStatus?)null
        };

        if (!target.HasValue)
            return Conflict(ErrorCodes.OrderInvalidStatus);

        await ApplyStatusChange(
            order,
            target.Value,
            userId,
            DateTime.UtcNow,
            setClosedAt: false);
        await db.SaveChangesAsync();
        return Success();
    }

    [HttpDelete("{orderId:int}")]
    public async Task<IActionResult> Delete([FromRoute] int orderId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (orderId <= 0)
            return BadRequest(ErrorCodes.InvalidRequest);

        var (networkId, userErr) = await GetUserNetworkId(userId);
        if (userErr != null)
            return userErr;

        var order = await db.Orders
            .Include(o => o.Establishment)
            .FirstOrDefaultAsync(o =>
                o.Id == orderId &&
                o.Establishment.NetworkId == networkId);
        if (order == null)
            return NotFound(ErrorCodes.OrderNotFound);

        var canDelete = await HasEstablishmentPermission(
            userId,
            order.EstablishmentId,
            PermissionType.OrdersDelete);
        if (!canDelete)
            return Forbidden(ErrorCodes.PermissionDenied);

        if (order.Status != OrderStatus.Cancelled)
            return Conflict(ErrorCodes.OrderDeleteForbidden);

        db.Orders.Remove(order);
        await db.SaveChangesAsync();
        return Success();
    }

    private async Task<(int networkId, IActionResult? error)> GetUserNetworkId(int userId)
    {
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return (0, Unauthorized(ErrorCodes.UserNotFound));

        if (!user.NetworkId.HasValue)
            return (0, NotFound(ErrorCodes.NetworkNotFound));

        return (user.NetworkId.Value, null);
    }

    private async Task<bool> HasEstablishmentPermission(
        int userId,
        int establishmentId,
        PermissionType permission)
    {
        return await PermissionUtils.HasEstablishmentPermission(
            db,
            userId,
            establishmentId,
            permission);
    }

    private async Task<(List<OrderItemEntity> items, decimal totalSum, IActionResult? error)> BuildOrderItems(
        int networkId,
        int establishmentId,
        List<OrderItemRequest> requestItems)
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
                    Price = pp.Price
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
                LineTotal = line
            });
        }

        return (items, total, null);
    }

    private async Task<IActionResult> ChangeStatusSimple(
        int orderId,
        OrderStatus toStatus,
        PermissionType permission,
        OrderStatus? onlyIfCurrent = null,
        bool setClosedAt = false,
        OrderStatus[]? allowFromStatuses = null)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (orderId <= 0)
            return BadRequest(ErrorCodes.InvalidRequest);

        var (networkId, userErr) = await GetUserNetworkId(userId);
        if (userErr != null)
            return userErr;

        var order = await db.Orders
            .Include(o => o.Establishment)
            .Include(o => o.Staff)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.Establishment.NetworkId == networkId);
        if (order == null)
            return NotFound(ErrorCodes.OrderNotFound);

        var can = await HasEstablishmentPermission(
            userId,
            order.EstablishmentId,
            permission);
        if (!can)
            return Forbidden(ErrorCodes.PermissionDenied);

        if (order.Status is OrderStatus.Completed or OrderStatus.Cancelled)
            return Conflict(ErrorCodes.OrderInvalidStatus);

        if (onlyIfCurrent.HasValue && order.Status != onlyIfCurrent.Value)
            return Conflict(ErrorCodes.OrderInvalidStatus);

        if (allowFromStatuses != null && !allowFromStatuses.Contains(order.Status))
            return Conflict(ErrorCodes.OrderInvalidStatus);

        await ApplyStatusChange(order, toStatus, userId, DateTime.UtcNow, setClosedAt);
        await db.SaveChangesAsync();
        return Success();
    }

    private async Task<IActionResult> ChangeStatusWithWaiterAssignment(
        int orderId,
        OrderStatus toStatus,
        PermissionType permission,
        OrderStatus onlyIfCurrent)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        var resp = await ChangeStatusSimple(
            orderId,
            toStatus,
            permission,
            onlyIfCurrent: onlyIfCurrent);
        if (resp is not OkObjectResult)
            return resp;

        var order = await db.Orders
            .Include(o => o.Staff)
            .FirstAsync(o => o.Id == orderId);

        if (!order.Staff.Any(s => s.Role == OrderStaffRole.Waiter))
        {
            order.Staff.Add(new OrderStaffEntity
            {
                Role = OrderStaffRole.Waiter,
                AssignedAt = DateTime.UtcNow,
                UserId = userId
            });
            await db.SaveChangesAsync();
        }

        return Success();
    }

    private async Task<IActionResult> ChangeStatusWithCookAssignment(
        int orderId,
        OrderStatus toStatus,
        PermissionType permission,
        OrderStatus onlyIfCurrent)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(ErrorCodes.UserNotFound);

        if (orderId <= 0)
            return BadRequest(ErrorCodes.InvalidRequest);

        var (networkId, userErr) = await GetUserNetworkId(userId);
        if (userErr != null)
            return userErr;

        var order = await db.Orders
            .Include(o => o.Establishment)
            .Include(o => o.Staff)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.Establishment.NetworkId == networkId);
        if (order == null)
            return NotFound(ErrorCodes.OrderNotFound);

        var can = await HasEstablishmentPermission(
            userId,
            order.EstablishmentId,
            permission);
        if (!can)
            return Forbidden(ErrorCodes.PermissionDenied);

        if (order.Status != onlyIfCurrent)
            return Conflict(ErrorCodes.OrderInvalidStatus);

        var now = DateTime.UtcNow;

        if (!order.Staff.Any(s => s.UserId == userId && s.Role == OrderStaffRole.Cook))
        {
            order.Staff.Add(new OrderStaffEntity
            {
                Role = OrderStaffRole.Cook,
                AssignedAt = now,
                UserId = userId
            });
        }

        await ApplyStatusChange(order, toStatus, userId, now, setClosedAt: false);
        await db.SaveChangesAsync();
        return Success();
    }

    private Task ApplyStatusChange(
        OrderEntity order,
        OrderStatus toStatus,
        int changedByUserId,
        DateTime now,
        bool setClosedAt)
    {
        var fromStatus = order.Status;
        order.Status = toStatus;

        if (setClosedAt)
            order.ClosedAt = now;

        db.OrderStatusHistories.Add(new OrderStatusHistoryEntity
        {
            OrderId = order.Id,
            FromStatus = fromStatus,
            ToStatus = toStatus,
            ChangedAt = now,
            ChangedByUserId = changedByUserId
        });

        return Task.CompletedTask;
    }
}
