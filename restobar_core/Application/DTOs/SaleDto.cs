namespace restobar_core.Application.DTOs;

public class SaleDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public DateTimeOffset RegisteredAt { get; set; }
    public string RegisteredBy { get; set; } = string.Empty;
}
