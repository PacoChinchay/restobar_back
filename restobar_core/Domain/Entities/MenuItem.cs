namespace restobar_core.Domain.Entities;

public class MenuItem
{
    public int Id { get; set; }
    public int MenuId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int InitialQuantity { get; set; }
    public int RemainingQuantity { get; set; }
    public Menu Menu { get; set; } = null!;
}
