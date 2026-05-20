using restobar_core.Application.DTOs;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class ActivateMenuUseCase(IMenuRepository repo)
{
    public Task<MenuDto> ExecuteAsync(int id) => repo.ActivateAsync(id);
}
