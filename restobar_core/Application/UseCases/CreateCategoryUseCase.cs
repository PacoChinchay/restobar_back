using restobar_core.Application.DTOs;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class CreateCategoryUseCase(ICategoryRepository repository)
{
    public async Task<CategoryDto> ExecuteAsync(CreateCategoryRequest request)
    {
        var category = await repository.CreateAsync(request.Name.Trim(), request.IsDrink);
        return new CategoryDto { Id = category.Id, Name = category.Name, IsDrink = category.IsDrink };
    }
}
