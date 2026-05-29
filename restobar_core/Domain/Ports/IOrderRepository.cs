using restobar_core.Application.DTOs;

namespace restobar_core.Domain.Ports;

public interface IOrderRepository
{
    Task<List<OrderDto>> GetOpenAsync();
    Task<OrderDto?> GetByIdAsync(int id);
    Task<OrderDto> CreateAsync(int tableNumber, List<OrderItemInput> items, string? createdBy);
    Task<OrderDto> AddItemAsync(int orderId, OrderItemInput item);
    Task RemoveItemAsync(int orderId, int itemId);
    Task<OrderDto> UpdateItemQuantityAsync(int orderId, int itemId, int quantity);
    Task<OrderDto> PayAsync(int orderId, string paymentMethod, string registeredBy);
    Task CancelAsync(int orderId);
    Task<WaiterWeekSummaryDto> GetWaiterWeekStatsAsync(DateOnly endDate);
}
