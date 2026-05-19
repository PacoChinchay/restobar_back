using restobar_core.Application.DTOs;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class GetOpenOrdersUseCase(IOrderRepository repo)
{
    public Task<List<OrderDto>> ExecuteAsync() => repo.GetOpenAsync();
}
