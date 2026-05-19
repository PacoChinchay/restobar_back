using restobar_core.Application.DTOs;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class UpdateOrderItemUseCase(IOrderRepository repo)
{
    public Task<OrderDto> ExecuteAsync(int orderId, int itemId, int quantity) =>
        repo.UpdateItemQuantityAsync(orderId, itemId, quantity);
}
