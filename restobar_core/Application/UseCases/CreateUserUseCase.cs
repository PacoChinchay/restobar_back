using restobar_core.Application.DTOs;
using restobar_core.Domain.Enums;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class CreateUserUseCase(IAuthPort authPort)
{
    public async Task<UserDto> ExecuteAsync(CreateUserRequest request)
    {
        if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var role))
            throw new ArgumentException($"Rol inválido: {request.Role}");

        var initials = ToInitials(request.Name);
        var user = await authPort.CreateUserAsync(request.Name, initials, role, request.Pin);
        return new UserDto { Id = user.Id, Name = user.Name, Initials = user.Initials, Role = user.Role.ToString() };
    }

    private static string ToInitials(string name) =>
        string.Concat(name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Take(2).Select(w => char.ToUpper(w[0])));
}
