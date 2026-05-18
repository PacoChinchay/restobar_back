using Microsoft.EntityFrameworkCore;
using restobar_core.Domain.Entities;
using restobar_core.Domain.Enums;
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

    public async Task<Product> SaveAsync(Product product)
    {
        db.Products.Add(product);
        await db.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateAsync(int id, string name, decimal price, ProductCategory category)
    {
        var product = await db.Products.FindAsync(id)
            ?? throw new KeyNotFoundException($"Producto {id} no encontrado.");

        product.Name = name;
        product.Price = price;
        product.Category = category;

        await db.SaveChangesAsync();
        return product;
    }
}
