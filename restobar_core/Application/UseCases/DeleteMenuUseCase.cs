using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class DeleteMenuUseCase(IMenuRepository repo)
{
    public Task ExecuteAsync(int id) => repo.DeleteAsync(id);
}
