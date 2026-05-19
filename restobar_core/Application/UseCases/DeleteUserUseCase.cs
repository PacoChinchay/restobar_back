using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class DeleteUserUseCase(IAuthPort authPort)
{
    public Task ExecuteAsync(string id) => authPort.DeleteUserAsync(id);
}
