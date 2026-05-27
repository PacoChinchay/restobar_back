namespace restobar_core.Application.DTOs;

public class CreateOrderRequest
{
    public int TableNumber { get; set; }
    public List<OrderItemInput> Items { get; set; } = new();
    public string? CreatedBy { get; set; }
}
