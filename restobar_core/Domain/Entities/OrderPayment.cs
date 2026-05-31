using restobar_core.Domain.Enums;

namespace restobar_core.Domain.Entities;

public class OrderPayment
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public PaymentMethod Method { get; set; }
    public decimal Amount { get; set; }
    public DateTime RegisteredAt { get; set; }
    public string RegisteredBy { get; set; } = string.Empty;
}
