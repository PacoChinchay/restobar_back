using restobar_core.Application.DTOs;
using restobar_core.Application.Services;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class CreateOrderUseCase(IOrderRepository repo, KitchenPrinterService printer)
{
    public async Task<OrderDto> ExecuteAsync(CreateOrderRequest request)
    {
        var order = await repo.CreateAsync(request.TableNumber, request.Items, request.CreatedBy);
        // Fire-and-forget: printer errors must never fail the HTTP response
        _ = printer.PrintKitchenTicketAsync(order, request.Items);
        return order;
    }
}
