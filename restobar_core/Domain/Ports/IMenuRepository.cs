using restobar_core.Application.DTOs;
using restobar_core.Domain.Enums;

namespace restobar_core.Domain.Ports;

public interface IMenuRepository
{
    Task<List<MenuDto>> GetAllAsync();
    Task<MenuDto?> GetActiveAsync(MenuType type = MenuType.daily);
    Task<MenuDto> CreateAsync(string name, MenuType type, List<MenuItemInput> items);
    Task<MenuDto> UpdateAsync(int id, string name, List<MenuItemInput> items);
    Task DeleteAsync(int id);
    Task<MenuDto> ActivateAsync(int id);
    Task DeactivateAsync(int id);
    Task<MenuDto> UpdateItemQuantityAsync(int menuId, int itemId, int newQuantity);
}
