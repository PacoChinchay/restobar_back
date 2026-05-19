using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class CancelOrderUseCase(IOrderRepository repo)
{
    public Task ExecuteAsync(int orderId) => repo.CancelAsync(orderId);
}
