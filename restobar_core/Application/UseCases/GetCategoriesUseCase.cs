using restobar_core.Application.DTOs;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class GetCategoriesUseCase(ICategoryRepository repository)
{
    public async Task<List<CategoryDto>> ExecuteAsync()
    {
        var categories = await repository.GetAllAsync();
        return categories.Select(c => new CategoryDto { Id = c.Id, Name = c.Name, IsDrink = c.IsDrink }).ToList();
    }
}
