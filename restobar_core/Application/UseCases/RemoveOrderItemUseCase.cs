using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class RemoveOrderItemUseCase(IOrderRepository repo)
{
    public Task ExecuteAsync(int orderId, int itemId) =>
        repo.RemoveItemAsync(orderId, itemId);
}
