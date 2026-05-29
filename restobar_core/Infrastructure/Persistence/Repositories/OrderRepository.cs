using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using restobar_core.Application.DTOs;
using restobar_core.Application.Hubs;
using restobar_core.Domain.Entities;
using restobar_core.Domain.Enums;
using restobar_core.Domain.Ports;
using restobar_core.Infrastructure.Persistence;

namespace restobar_core.Infrastructure.Persistence.Repositories;

public class OrderRepository(AppDbContext db, IHubContext<StockHub> hub) : IOrderRepository
{
    private static readonly TimeZoneInfo LimaZone =
        TimeZoneInfo.FindSystemTimeZoneById("America/Lima");

    private static OrderDto ToDto(Order o) => new()
    {
        Id = o.Id,
        TableNumber = o.TableNumber,
        Status = o.Status,
        Items = o.Items.Select(i => new OrderItemDto
        {
            Id = i.Id,
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            UnitPrice = i.UnitPrice,
            Quantity = i.Quantity,
            Subtotal = i.Subtotal,
        }).ToList(),
        TotalAmount = o.Items.Sum(i => i.Subtotal),
        CreatedAt = new DateTimeOffset(o.CreatedAt, TimeSpan.Zero),
        PaidAt = o.PaidAt.HasValue ? new DateTimeOffset(o.PaidAt.Value, TimeSpan.Zero) : null,
        PaymentMethod = o.PaymentMethod,
        CreatedBy = o.CreatedBy,
    };

    public async Task<List<OrderDto>> GetOpenAsync()
    {
        var orders = await db.Orders
            .Include(o => o.Items)
            .Where(o => o.Status == "open")
            .OrderBy(o => o.CreatedAt)
            .ToListAsync();
        return orders.Select(ToDto).ToList();
    }

    public async Task<OrderDto?> GetByIdAsync(int id)
    {
        var order = await db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);
        return order is null ? null : ToDto(order);
    }

    private async Task<StockUpdateDto?> DecrementMenuStock(int productId, int quantity)
    {
        var menuItem = await db.MenuItems
            .Include(mi => mi.Menu)
            .FirstOrDefaultAsync(mi => mi.Menu.IsActive && mi.ProductId == productId);
        if (menuItem is null) return null;
        menuItem.RemainingQuantity = Math.Max(0, menuItem.RemainingQuantity - quantity);
        return new StockUpdateDto(productId, menuItem.RemainingQuantity);
    }

    private async Task<StockUpdateDto?> RestoreMenuStock(int productId, int quantity)
    {
        var menuItem = await db.MenuItems
            .Include(mi => mi.Menu)
            .FirstOrDefaultAsync(mi => mi.Menu.IsActive && mi.ProductId == productId);
        if (menuItem is null) return null;
        menuItem.RemainingQuantity = Math.Min(menuItem.InitialQuantity, menuItem.RemainingQuantity + quantity);
        return new StockUpdateDto(productId, menuItem.RemainingQuantity);
    }

    private async Task BroadcastStock(IEnumerable<StockUpdateDto?> updates)
    {
        var valid = updates.Where(u => u is not null).Cast<StockUpdateDto>().ToList();
        if (valid.Count > 0)
            await hub.Clients.All.SendAsync("StockUpdated", valid);
    }

    public async Task<OrderDto> CreateAsync(int tableNumber, List<OrderItemInput> items, string? createdBy)
    {
        var order = new Order
        {
            TableNumber = tableNumber,
            Status = "open",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
            Items = items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                Subtotal = i.UnitPrice * i.Quantity,
            }).ToList(),
        };
        db.Orders.Add(order);
        var updates = new List<StockUpdateDto?>();
        foreach (var item in items)
            updates.Add(await DecrementMenuStock(item.ProductId, item.Quantity));
        await db.SaveChangesAsync();
        await BroadcastStock(updates);
        return ToDto(order);
    }

    public async Task<OrderDto> AddItemAsync(int orderId, OrderItemInput item)
    {
        var order = await db.Orders.Include(o => o.Items).FirstAsync(o => o.Id == orderId);
        var existing = order.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
        if (existing is not null)
        {
            existing.Quantity += item.Quantity;
            existing.Subtotal = existing.UnitPrice * existing.Quantity;
        }
        else
        {
            order.Items.Add(new OrderItem
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                Subtotal = item.UnitPrice * item.Quantity,
            });
        }
        var update = await DecrementMenuStock(item.ProductId, item.Quantity);
        await db.SaveChangesAsync();
        await BroadcastStock([update]);
        return ToDto(order);
    }

    public async Task RemoveItemAsync(int orderId, int itemId)
    {
        var item = await db.OrderItems.FirstAsync(i => i.Id == itemId && i.OrderId == orderId);
        var update = await RestoreMenuStock(item.ProductId, item.Quantity);
        db.OrderItems.Remove(item);
        await db.SaveChangesAsync();
        await BroadcastStock([update]);
    }

    public async Task<OrderDto> UpdateItemQuantityAsync(int orderId, int itemId, int quantity)
    {
        var order = await db.Orders.Include(o => o.Items).FirstAsync(o => o.Id == orderId);
        var item = order.Items.First(i => i.Id == itemId);
        var diff = quantity - item.Quantity;
        StockUpdateDto? update = null;
        if (diff > 0)
            update = await DecrementMenuStock(item.ProductId, diff);
        else if (diff < 0)
            update = await RestoreMenuStock(item.ProductId, -diff);
        item.Quantity = quantity;
        item.Subtotal = item.UnitPrice * quantity;
        await db.SaveChangesAsync();
        if (update is not null) await BroadcastStock([update]);
        return ToDto(order);
    }

    public async Task<OrderDto> PayAsync(int orderId, string paymentMethod, string registeredBy)
    {
        var order = await db.Orders.Include(o => o.Items).FirstAsync(o => o.Id == orderId);
        if (!Enum.TryParse<PaymentMethod>(paymentMethod, ignoreCase: true, out var method))
            throw new ArgumentException($"Invalid payment method: {paymentMethod}");

        var now = DateTime.UtcNow;
        foreach (var item in order.Items)
        {
            db.Sales.Add(new Sale
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                PaymentMethod = method,
                Total = item.Subtotal,
                RegisteredAt = now,
                RegisteredBy = registeredBy,
            });
        }

        order.Status = "paid";
        order.PaidAt = now;
        order.PaymentMethod = paymentMethod;
        await db.SaveChangesAsync();
        return ToDto(order);
    }

    public async Task CancelAsync(int orderId)
    {
        var order = await db.Orders.FirstAsync(o => o.Id == orderId);
        order.Status = "cancelled";
        await db.SaveChangesAsync();
    }

    public async Task<WaiterWeekSummaryDto> GetWaiterWeekStatsAsync(DateOnly endDate)
    {
        var startDate = endDate.AddDays(-6);
        var startUtc  = TimeZoneInfo.ConvertTimeToUtc(startDate.ToDateTime(TimeOnly.MinValue), LimaZone);
        var endUtc    = TimeZoneInfo.ConvertTimeToUtc(endDate.ToDateTime(new TimeOnly(23, 59, 59)), LimaZone);

        var orders = await db.Orders
            .Include(o => o.Items)
            .Where(o => o.CreatedAt >= startUtc && o.CreatedAt <= endUtc && o.CreatedBy != null)
            .ToListAsync();

        var dailyTotals = Enumerable.Range(0, 7)
            .Select(i =>
            {
                var day  = startDate.AddDays(i);
                var s    = TimeZoneInfo.ConvertTimeToUtc(day.ToDateTime(TimeOnly.MinValue), LimaZone);
                var e    = TimeZoneInfo.ConvertTimeToUtc(day.ToDateTime(new TimeOnly(23, 59, 59)), LimaZone);
                var cnt  = orders.Count(o => o.CreatedAt >= s && o.CreatedAt <= e);
                return new DailyOrderCountDto(day.ToString("yyyy-MM-dd"), cnt);
            })
            .ToList();

        var waiters = orders
            .GroupBy(o => o.CreatedBy!)
            .Select(g =>
            {
                var paid    = g.Where(o => o.Status == "paid").ToList();
                var revenue = paid.Sum(o => o.Items.Sum(i => i.Subtotal));
                var cnt     = g.Count();
                return new WaiterStatsDto(
                    g.Key,
                    cnt,
                    g.Select(o => o.TableNumber).Distinct().Count(),
                    revenue,
                    paid.Count > 0 ? revenue / paid.Count : 0
                );
            })
            .OrderByDescending(w => w.TotalOrders)
            .ToList();

        return new WaiterWeekSummaryDto(waiters, dailyTotals);
    }
}
