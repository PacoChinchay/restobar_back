using restobar_core.Application.DTOs;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class CreateOrderUseCase(IOrderRepository repo)
{
    public Task<OrderDto> ExecuteAsync(CreateOrderRequest request) =>
        repo.CreateAsync(request.TableNumber, request.Items);
}
