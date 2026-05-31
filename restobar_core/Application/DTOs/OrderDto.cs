namespace restobar_core.Application.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public int TableNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<OrderItemDto> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? PaidAt { get; set; }
    public string? PaymentMethod { get; set; }
    public string? CreatedBy { get; set; }
    public List<OrderPaymentDto> Payments { get; set; } = [];
}
