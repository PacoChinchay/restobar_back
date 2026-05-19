using Microsoft.EntityFrameworkCore;
using restobar_core.Domain.Entities;
using restobar_core.Domain.Ports;
using restobar_core.Infrastructure.Persistence;

namespace restobar_core.Infrastructure.Persistence.Repositories;

public class CategoryRepository(AppDbContext db) : ICategoryRepository
{
    public async Task<List<Category>> GetAllAsync()
    {
        return await db.Categories.OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<Category> CreateAsync(string name)
    {
        var category = new Category { Name = name };
        db.Categories.Add(category);
        await db.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateAsync(int id, string name)
    {
        var category = await db.Categories.FindAsync(id)
            ?? throw new KeyNotFoundException($"Categoría {id} no encontrada.");

        category.Name = name;
        await db.SaveChangesAsync();
        return category;
    }

    public async Task DeleteAsync(int id)
    {
        var category = await db.Categories.FindAsync(id)
            ?? throw new KeyNotFoundException($"Categoría {id} no encontrada.");

        db.Categories.Remove(category);
        await db.SaveChangesAsync();
    }

    public async Task<bool> IsInUseAsync(int id)
    {
        var category = await db.Categories.FindAsync(id);
        if (category is null) return false;

        return await db.Products.AnyAsync(p => p.Active && p.Category == category.Name);
    }
}
