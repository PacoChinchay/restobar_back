using restobar_core.Application.DTOs;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class GetMenusUseCase(IMenuRepository repo)
{
    public Task<List<MenuDto>> ExecuteAsync() => repo.GetAllAsync();
}
