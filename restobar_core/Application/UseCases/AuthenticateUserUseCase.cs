using restobar_core.Application.DTOs;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class AuthenticateUserUseCase(IAuthPort authPort)
{
    public async Task<List<UserDto>> GetUsersAsync()
    {
        var users = await authPort.GetUsersAsync();
        return users.Select(u => new UserDto
        {
            Id = u.Id,
            Name = u.Name,
            Initials = u.Initials,
            Role = u.Role.ToString()
        }).ToList();
    }

    public async Task<UserDto?> ValidatePinAsync(string userId, string pin)
    {
        var user = await authPort.ValidatePinAsync(userId, pin);
        if (user is null) return null;

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Initials = user.Initials,
            Role = user.Role.ToString()
        };
    }
}
