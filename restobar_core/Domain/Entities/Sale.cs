using restobar_core.Domain.Enums;

namespace restobar_core.Domain.Entities;

public class Sale
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public decimal Total { get; set; }
    public DateTime RegisteredAt { get; set; }
    public string RegisteredBy { get; set; } = string.Empty;

    public Product Product { get; set; } = null!;
}
