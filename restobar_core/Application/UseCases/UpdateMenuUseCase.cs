using restobar_core.Application.DTOs;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class UpdateMenuUseCase(IMenuRepository repo)
{
    public Task<MenuDto> ExecuteAsync(int id, CreateMenuRequest request) =>
        repo.UpdateAsync(id, request.Name, request.Items);
}
