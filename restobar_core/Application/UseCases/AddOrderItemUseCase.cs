using restobar_core.Application.DTOs;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class AddOrderItemUseCase(IOrderRepository repo)
{
    public Task<OrderDto> ExecuteAsync(int orderId, OrderItemInput item) =>
        repo.AddItemAsync(orderId, item);
}
