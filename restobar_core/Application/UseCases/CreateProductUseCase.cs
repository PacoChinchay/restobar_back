using restobar_core.Application.DTOs;
using restobar_core.Domain.Entities;
using restobar_core.Domain.Enums;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class CreateProductUseCase(IProductRepository repository)
{
    public async Task<ProductDto> ExecuteAsync(CreateProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name,
            Price = request.Price,
            Category = Enum.Parse<ProductCategory>(request.Category, ignoreCase: true),
        };

        var saved = await repository.SaveAsync(product);

        return new ProductDto
        {
            Id = saved.Id,
            Name = saved.Name,
            Price = saved.Price,
            Category = saved.Category.ToString(),
            Active = saved.Active
        };
    }
}
