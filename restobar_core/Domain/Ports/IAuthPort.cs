using restobar_core.Domain.Entities;

namespace restobar_core.Domain.Ports;

public interface IAuthPort
{
    Task<List<User>> GetUsersAsync();
    Task<User?> ValidatePinAsync(string userId, string pin);
}
