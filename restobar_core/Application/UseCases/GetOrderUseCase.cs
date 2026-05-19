using restobar_core.Application.DTOs;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class GetOrderUseCase(IOrderRepository repo)
{
    public Task<OrderDto?> ExecuteAsync(int id) => repo.GetByIdAsync(id);
}
