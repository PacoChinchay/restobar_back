using restobar_core.Application.DTOs;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class UpdateCategoryUseCase(ICategoryRepository repository)
{
    public async Task<CategoryDto> ExecuteAsync(int id, UpdateCategoryRequest request)
    {
        var category = await repository.UpdateAsync(id, request.Name.Trim());
        return new CategoryDto { Id = category.Id, Name = category.Name };
    }
}
