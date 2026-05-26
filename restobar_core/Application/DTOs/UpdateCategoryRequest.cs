namespace restobar_core.Application.DTOs;

public class UpdateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public bool IsDrink { get; set; } = false;
}
