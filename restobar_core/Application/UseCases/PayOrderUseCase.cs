using restobar_core.Application.DTOs;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class PayOrderUseCase(IOrderRepository repo)
{
    public Task<OrderDto> ExecuteAsync(int orderId, PayOrderRequest request) =>
        repo.PayAsync(orderId, request.PaymentMethod, request.RegisteredBy);
}
