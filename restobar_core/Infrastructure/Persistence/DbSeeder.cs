using restobar_core.Domain.Entities;
using restobar_core.Domain.Enums;

namespace restobar_core.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (!db.Categories.Any())
        {
            db.Categories.AddRange(
                new Category { Name = "Platos" },
                new Category { Name = "Bebidas" },
                new Category { Name = "Postres" }
            );
            await db.SaveChangesAsync();
        }

        if (!db.Products.Any())
        {
            var products = new List<Product>
            {
                new() { Name = "Lomo Saltado",    Price = 18m, Category = "Platos",  Active = true },
                new() { Name = "Ceviche",         Price = 22m, Category = "Platos",  Active = true },
                new() { Name = "Arroz con Pollo", Price = 15m, Category = "Platos",  Active = true },
                new() { Name = "Ají de Gallina",  Price = 16m, Category = "Platos",  Active = true },
                new() { Name = "Chicharrón",      Price = 20m, Category = "Platos",  Active = true },
                new() { Name = "Aguadito",        Price = 14m, Category = "Platos",  Active = true },
                new() { Name = "Gaseosa",         Price = 5m,  Category = "Bebidas", Active = true },
                new() { Name = "Cerveza",         Price = 8m,  Category = "Bebidas", Active = true },
                new() { Name = "Agua",            Price = 3m,  Category = "Bebidas", Active = true },
                new() { Name = "Arroz con Leche", Price = 7m,  Category = "Postres", Active = true },
            };

            db.Products.AddRange(products);
        }

        if (!db.Users.Any())
        {
            var users = new List<User>
            {
                new()
                {
                    Id = "admin",
                    Name = "Administrador",
                    Initials = "AD",
                    Role = UserRole.admin,
                    PinHash = BCrypt.Net.BCrypt.HashPassword("1234", workFactor: 11)
                },
                new()
                {
                    Id = "cajero",
                    Name = "Cajero",
                    Initials = "CA",
                    Role = UserRole.cajero,
                    PinHash = BCrypt.Net.BCrypt.HashPassword("5678", workFactor: 11)
                }
            };

            db.Users.AddRange(users);
        }

        await db.SaveChangesAsync();
    }
}
