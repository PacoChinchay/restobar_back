using Microsoft.EntityFrameworkCore;
using restobar_core.Domain.Entities;
using restobar_core.Domain.Ports;
using restobar_core.Infrastructure.Persistence;

namespace restobar_core.Infrastructure.Persistence.Repositories;

public class AuthRepository(AppDbContext db) : IAuthPort
{
    public async Task<List<User>> GetUsersAsync()
    {
        return await db.Users.ToListAsync();
    }

    public async Task<User?> ValidatePinAsync(string userId, string pin)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null) return null;

        return BCrypt.Net.BCrypt.Verify(pin, user.PinHash) ? user : null;
    }
}
