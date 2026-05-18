using restobar_core.Application.DTOs;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class GetProductsUseCase(IProductRepository repository)
{
    public async Task<List<ProductDto>> ExecuteAsync()
    {
        var products = await repository.GetActiveAsync();
        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Category = p.Category.ToString(),
            Active = p.Active
        }).ToList();
    }

    public async Task<List<ProductDto>> ExecuteAllAsync()
    {
        var products = await repository.GetAllAsync();
        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Category = p.Category.ToString(),
            Active = p.Active
        }).ToList();
    }
}
