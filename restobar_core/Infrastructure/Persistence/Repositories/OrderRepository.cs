using Microsoft.EntityFrameworkCore;
using restobar_core.Application.DTOs;
using restobar_core.Domain.Entities;
using restobar_core.Domain.Enums;
using restobar_core.Domain.Ports;
using restobar_core.Infrastructure.Persistence;

namespace restobar_core.Infrastructure.Persistence.Repositories;

public class OrderRepository(AppDbContext db) : IOrderRepository
{
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

    public async Task<OrderDto> CreateAsync(int tableNumber, List<OrderItemInput> items)
    {
        var order = new Order
        {
            TableNumber = tableNumber,
            Status = "open",
            CreatedAt = DateTime.UtcNow,
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
        await db.SaveChangesAsync();
        return ToDto(order);
    }

    public async Task RemoveItemAsync(int orderId, int itemId)
    {
        var item = await db.OrderItems.FirstAsync(i => i.Id == itemId && i.OrderId == orderId);
        db.OrderItems.Remove(item);
        await db.SaveChangesAsync();
    }

    public async Task<OrderDto> UpdateItemQuantityAsync(int orderId, int itemId, int quantity)
    {
        var order = await db.Orders.Include(o => o.Items).FirstAsync(o => o.Id == orderId);
        var item = order.Items.First(i => i.Id == itemId);
        item.Quantity = quantity;
        item.Subtotal = item.UnitPrice * quantity;
        await db.SaveChangesAsync();
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
}
