using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class DeactivateMenuUseCase(IMenuRepository repo)
{
    public Task ExecuteAsync(int id) => repo.DeactivateAsync(id);
}
