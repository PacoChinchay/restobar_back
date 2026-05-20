namespace restobar_core.Application.DTOs;

public class MenuDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<MenuItemDto> Items { get; set; } = new();
}

public class MenuItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int InitialQuantity { get; set; }
    public int RemainingQuantity { get; set; }
}

public class MenuItemInput
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}

public class CreateMenuRequest
{
    public string Name { get; set; } = string.Empty;
    public List<MenuItemInput> Items { get; set; } = new();
}

public class UpdateMenuItemQuantityRequest
{
    public int Quantity { get; set; }
}
