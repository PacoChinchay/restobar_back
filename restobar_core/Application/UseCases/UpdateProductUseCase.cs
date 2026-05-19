using restobar_core.Application.DTOs;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class UpdateProductUseCase(IProductRepository repository)
{
    public async Task<ProductDto> ExecuteAsync(int id, UpdateProductRequest request)
    {
        var updated = await repository.UpdateAsync(id, request.Name, request.Price, request.Category);

        return new ProductDto
        {
            Id = updated.Id,
            Name = updated.Name,
            Price = updated.Price,
            Category = updated.Category,
            Active = updated.Active
        };
    }
}
