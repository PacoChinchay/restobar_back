using restobar_core.Application.DTOs;
using restobar_core.Domain.Enums;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class GetActiveMenuUseCase(IMenuRepository repo)
{
    public Task<MenuDto?> ExecuteAsync(MenuType type = MenuType.daily) => repo.GetActiveAsync(type);
}
