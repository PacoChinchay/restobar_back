using Microsoft.EntityFrameworkCore;
using restobar_core.Domain.Entities;
using restobar_core.Domain.Ports;
using restobar_core.Infrastructure.Persistence;

namespace restobar_core.Infrastructure.Persistence.Repositories;

public class ProductRepository(AppDbContext db) : IProductRepository
{
    public async Task<List<Product>> GetAllAsync()
    {
        return await db.Products
            .OrderBy(p => p.Category)
            .ThenBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<List<Product>> GetActiveAsync()
    {
        return await db.Products
            .Where(p => p.Active)
            .OrderBy(p => p.Category)
            .ThenBy(p => p.Name)
            .ToListAsync();
    }
}
