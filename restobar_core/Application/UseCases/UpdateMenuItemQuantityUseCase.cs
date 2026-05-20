using restobar_core.Application.DTOs;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class UpdateMenuItemQuantityUseCase(IMenuRepository repo)
{
    public Task<MenuDto> ExecuteAsync(int menuId, int itemId, int newQuantity) =>
        repo.UpdateItemQuantityAsync(menuId, itemId, newQuantity);
}
