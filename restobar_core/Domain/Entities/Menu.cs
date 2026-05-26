using restobar_core.Domain.Enums;

namespace restobar_core.Domain.Entities;

public class Menu
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public MenuType Type { get; set; } = MenuType.daily;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<MenuItem> Items { get; set; } = new();
}
