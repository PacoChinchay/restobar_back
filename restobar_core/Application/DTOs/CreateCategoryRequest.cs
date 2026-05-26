namespace restobar_core.Application.DTOs;

public class CreateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public bool IsDrink { get; set; } = false;
}
