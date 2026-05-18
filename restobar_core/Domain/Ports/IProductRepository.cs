using restobar_core.Domain.Entities;

namespace restobar_core.Domain.Ports;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<List<Product>> GetActiveAsync();
}
