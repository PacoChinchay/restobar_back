using restobar_core.Application.DTOs;

namespace restobar_core.Domain.Ports;

public interface IMenuRepository
{
    Task<List<MenuDto>> GetAllAsync();
    Task<MenuDto?> GetActiveAsync();
    Task<MenuDto> CreateAsync(string name, List<MenuItemInput> items);
    Task<MenuDto> UpdateAsync(int id, string name, List<MenuItemInput> items);
    Task DeleteAsync(int id);
    Task<MenuDto> ActivateAsync(int id);
    Task DeactivateAsync(int id);
    Task<MenuDto> UpdateItemQuantityAsync(int menuId, int itemId, int newQuantity);
}
