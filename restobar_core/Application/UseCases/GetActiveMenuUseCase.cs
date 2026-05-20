using restobar_core.Application.DTOs;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class GetActiveMenuUseCase(IMenuRepository repo)
{
    public Task<MenuDto?> ExecuteAsync() => repo.GetActiveAsync();
}
