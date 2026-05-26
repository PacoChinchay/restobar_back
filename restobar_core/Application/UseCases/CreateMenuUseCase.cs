using restobar_core.Application.DTOs;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class CreateMenuUseCase(IMenuRepository repo)
{
    public Task<MenuDto> ExecuteAsync(CreateMenuRequest request) =>
        repo.CreateAsync(request.Name, request.Type, request.Items);
}
