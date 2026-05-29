using Microsoft.EntityFrameworkCore;
using restobar_core.Domain.Entities;
using restobar_core.Domain.Enums;
using restobar_core.Domain.Ports;
using restobar_core.Infrastructure.Persistence;

namespace restobar_core.Infrastructure.Persistence.Repositories;

public class AuthRepository(AppDbContext db) : IAuthPort
{
    public async Task<List<User>> GetUsersAsync() =>
        await db.Users.OrderBy(u => u.Name).ToListAsync();

    public async Task<User?> ValidatePinAsync(string userId, string pin)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null) return null;
        return BCrypt.Net.BCrypt.Verify(pin, user.PinHash) ? user : null;
    }

    public async Task<User> CreateUserAsync(string name, string initials, UserRole role, string pin, decimal? monthlySalary = null)
    {
        var user = new User
        {
            Id = Guid.NewGuid().ToString("N")[..12],
            Name = name,
            Initials = initials,
            Role = role,
            PinHash = BCrypt.Net.BCrypt.HashPassword(pin, workFactor: 11),
            MonthlySalary = monthlySalary,
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateUserAsync(string id, string name, string initials, UserRole role, string? pin, decimal? monthlySalary = null)
    {
        var user = await db.Users.FirstAsync(u => u.Id == id);
        user.Name = name;
        user.Initials = initials;
        user.Role = role;
        user.MonthlySalary = monthlySalary;
        if (!string.IsNullOrWhiteSpace(pin))
            user.PinHash = BCrypt.Net.BCrypt.HashPassword(pin, workFactor: 11);
        await db.SaveChangesAsync();
        return user;
    }

    public async Task DeleteUserAsync(string id)
    {
        var user = await db.Users.FirstAsync(u => u.Id == id);
        db.Users.Remove(user);
        await db.SaveChangesAsync();
    }
}
