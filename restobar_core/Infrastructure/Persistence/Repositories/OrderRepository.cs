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
        Payments = o.Payments.Select(p => new OrderPaymentDto(p.Method.ToString(), p.Amount)).ToList(),
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
        await using var tx = await db.Database.BeginTransactionAsync();

        // Atomic decrement: WHERE remaining >= qty ensures no overselling even under concurrency.
        // ExecuteUpdateAsync runs a single SQL UPDATE — the WHERE acts as the lock guard.
        foreach (var item in items)
        {
            var rows = await db.MenuItems
                .Where(mi => mi.Menu.IsActive && mi.ProductId == item.ProductId && mi.RemainingQuantity >= item.Quantity)
                .ExecuteUpdateAsync(s => s.SetProperty(mi => mi.RemainingQuantity, mi => mi.RemainingQuantity - item.Quantity));

            if (rows == 0)
            {
                // Distinguish "not in active menu" (ok) from "insufficient stock" (error)
                var tracked = await db.MenuItems
                    .AsNoTracking()
                    .Include(mi => mi.Menu)
                    .FirstOrDefaultAsync(mi => mi.Menu.IsActive && mi.ProductId == item.ProductId);

                if (tracked is not null)
                {
                    var avail = tracked.RemainingQuantity;
                    throw new InvalidOperationException(
                        $"Stock insuficiente para «{item.ProductName}». " +
                        (avail == 0 ? "Ya no quedan unidades." : $"Solo quedan {avail} unidad{(avail != 1 ? "es" : "")}."));
                }
            }
        }

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
        await db.SaveChangesAsync();
        await tx.CommitAsync();

        // Broadcast fresh stock values to all connected clients
        var updates = new List<StockUpdateDto?>();
        foreach (var item in items)
        {
            var mi = await db.MenuItems.AsNoTracking()
                .Include(x => x.Menu)
                .FirstOrDefaultAsync(x => x.Menu.IsActive && x.ProductId == item.ProductId);
            if (mi is not null) updates.Add(new StockUpdateDto(item.ProductId, mi.RemainingQuantity));
        }
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

    public async Task<OrderDto> PayAsync(int orderId, List<PaymentEntry> payments, string registeredBy)
    {
        if (payments.Count == 0)
            throw new ArgumentException("Se requiere al menos un método de pago.");

        var order = await db.Orders
            .Include(o => o.Items)
            .Include(o => o.Payments)
            .FirstAsync(o => o.Id == orderId);

        var total = order.Items.Sum(i => i.Subtotal);
        var paid  = payments.Sum(p => p.Amount);
        if (paid < total - 0.005m)
            throw new InvalidOperationException($"El monto pagado (S/ {paid:F2}) es menor al total (S/ {total:F2}).");

        var now = DateTime.UtcNow;

        // Register each payment entry
        foreach (var entry in payments)
        {
            if (!Enum.TryParse<PaymentMethod>(entry.Method, ignoreCase: true, out var parsedMethod))
                throw new ArgumentException($"Método de pago inválido: {entry.Method}");

            order.Payments.Add(new OrderPayment
            {
                Method = parsedMethod,
                Amount = entry.Amount,
                RegisteredAt = now,
                RegisteredBy = registeredBy,
            });
        }

        // Determine display label: single method or "mixto"
        var distinctMethods = payments.Select(p => p.Method.ToLower()).Distinct().ToList();
        order.PaymentMethod = distinctMethods.Count == 1 ? distinctMethods[0] : "mixto";
        order.Status = "paid";
        order.PaidAt = now;

        // Sales records use the dominant method (highest amount) for product-level tracking
        Enum.TryParse<PaymentMethod>(
            payments.MaxBy(p => p.Amount)!.Method,
            ignoreCase: true, out var dominantMethod);

        foreach (var item in order.Items)
        {
            db.Sales.Add(new Sale
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                PaymentMethod = dominantMethod,
                Total = item.Subtotal,
                RegisteredAt = now,
                RegisteredBy = registeredBy,
            });
        }

        // CashMovement only for the efectivo portion
        var cashAmount = payments
            .Where(p => string.Equals(p.Method, "efectivo", StringComparison.OrdinalIgnoreCase))
            .Sum(p => p.Amount);

        if (cashAmount > 0)
        {
            var limaDate = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(now, LimaZone));
            var session = await db.CashSessions.FirstOrDefaultAsync(s => s.Date == limaDate);
            if (session is not null)
            {
                db.CashMovements.Add(new CashMovement
                {
                    CashSessionId = session.Id,
                    MovementType = "ingreso",
                    Amount = cashAmount,
                    Description = $"Comanda #{orderId}",
                    CreatedAt = now,
                    CreatedBy = registeredBy,
                });
            }
        }

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

    public async Task<WaiterDaySummaryDto> GetWaiterDayStatsAsync(DateOnly date)
    {
        var startUtc = TimeZoneInfo.ConvertTimeToUtc(date.ToDateTime(TimeOnly.MinValue), LimaZone);
        var endUtc   = TimeZoneInfo.ConvertTimeToUtc(date.ToDateTime(new TimeOnly(23, 59, 59)), LimaZone);

        var orders = await db.Orders
            .Include(o => o.Items)
            .Where(o => o.CreatedAt >= startUtc && o.CreatedAt <= endUtc && o.CreatedBy != null)
            .ToListAsync();

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

        return new WaiterDaySummaryDto(date.ToString("yyyy-MM-dd"), waiters);
    }
}
