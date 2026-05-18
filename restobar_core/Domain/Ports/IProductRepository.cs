using restobar_core.Domain.Entities;
using restobar_core.Domain.Enums;

namespace restobar_core.Domain.Ports;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<List<Product>> GetActiveAsync();
    Task<Product> SaveAsync(Product product);
    Task<Product> UpdateAsync(int id, string name, decimal price, ProductCategory category);
}
