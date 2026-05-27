namespace restobar_core.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public int TableNumber { get; set; }
    public string Status { get; set; } = "open";
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? PaymentMethod { get; set; }
    public string? CreatedBy { get; set; }
    public List<OrderItem> Items { get; set; } = new();
}
