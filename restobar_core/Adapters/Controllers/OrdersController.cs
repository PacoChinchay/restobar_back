using Microsoft.AspNetCore.Mvc;
using restobar_core.Application.DTOs;
using restobar_core.Application.UseCases;

namespace restobar_core.Adapters.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(
    GetOpenOrdersUseCase getOpenOrders,
    GetOrderUseCase getOrder,
    CreateOrderUseCase createOrder,
    AddOrderItemUseCase addOrderItem,
    RemoveOrderItemUseCase removeOrderItem,
    UpdateOrderItemUseCase updateOrderItem,
    PayOrderUseCase payOrder,
    CancelOrderUseCase cancelOrder) : ControllerBase
{
    [HttpGet("open")]
    public async Task<ActionResult<List<OrderDto>>> GetOpen() =>
        Ok(await getOpenOrders.ExecuteAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var result = await getOrder.ExecuteAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderRequest request)
    {
        try
        {
            var result = await createOrder.ExecuteAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/items")]
    public async Task<ActionResult<OrderDto>> AddItem(int id, [FromBody] OrderItemInput request) =>
        Ok(await addOrderItem.ExecuteAsync(id, request));

    [HttpDelete("{id}/items/{itemId}")]
    public async Task<IActionResult> RemoveItem(int id, int itemId)
    {
        await removeOrderItem.ExecuteAsync(id, itemId);
        return NoContent();
    }

    [HttpPatch("{id}/items/{itemId}")]
    public async Task<ActionResult<OrderDto>> UpdateItem(int id, int itemId, [FromBody] UpdateOrderItemRequest request) =>
        Ok(await updateOrderItem.ExecuteAsync(id, itemId, request.Quantity));

    [HttpPost("{id}/pay")]
    public async Task<ActionResult<OrderDto>> Pay(int id, [FromBody] PayOrderRequest request) =>
        Ok(await payOrder.ExecuteAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Cancel(int id)
    {
        await cancelOrder.ExecuteAsync(id);
        return NoContent();
    }
}
