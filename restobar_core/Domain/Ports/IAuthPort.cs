using restobar_core.Domain.Entities;
using restobar_core.Domain.Enums;

namespace restobar_core.Domain.Ports;

public interface IAuthPort
{
    Task<List<User>> GetUsersAsync();
    Task<User?> ValidatePinAsync(string userId, string pin);
    Task<User> CreateUserAsync(string name, string initials, UserRole role, string pin);
    Task<User> UpdateUserAsync(string id, string name, string initials, UserRole role, string? pin);
    Task DeleteUserAsync(string id);
}
