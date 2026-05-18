using restobar_core.Domain.Enums;

namespace restobar_core.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public ProductCategory Category { get; set; }
    public bool Active { get; set; } = true;
}
