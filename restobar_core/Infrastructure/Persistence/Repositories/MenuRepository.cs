using Microsoft.EntityFrameworkCore;
using restobar_core.Application.DTOs;
using restobar_core.Domain.Entities;
using restobar_core.Domain.Enums;
using restobar_core.Domain.Ports;
using restobar_core.Infrastructure.Persistence;

namespace restobar_core.Infrastructure.Persistence.Repositories;

public class MenuRepository(AppDbContext db) : IMenuRepository
{
    private static MenuDto ToDto(Menu m) => new()
    {
        Id = m.Id,
        Name = m.Name,
        Type = m.Type,
        IsActive = m.IsActive,
        CreatedAt = m.CreatedAt,
        Items = m.Items.Select(i => new MenuItemDto
        {
            Id = i.Id,
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            UnitPrice = i.UnitPrice,
            InitialQuantity = i.InitialQuantity,
            RemainingQuantity = i.RemainingQuantity,
        }).ToList(),
    };

    public async Task<List<MenuDto>> GetAllAsync()
    {
        var menus = await db.Menus
            .Include(m => m.Items)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
        return menus.Select(ToDto).ToList();
    }

    public async Task<MenuDto?> GetActiveAsync(MenuType type = MenuType.daily)
    {
        var menu = await db.Menus
            .Include(m => m.Items)
            .FirstOrDefaultAsync(m => m.IsActive && m.Type == type);
        return menu is null ? null : ToDto(menu);
    }

    public async Task<MenuDto> CreateAsync(string name, MenuType type, List<MenuItemInput> items)
    {
        var menu = new Menu
        {
            Name = name,
            Type = type,
            IsActive = false,
            CreatedAt = DateTime.UtcNow,
            Items = items.Select(i => new MenuItem
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                InitialQuantity = i.Quantity,
                RemainingQuantity = i.Quantity,
            }).ToList(),
        };
        db.Menus.Add(menu);
        await db.SaveChangesAsync();
        return ToDto(menu);
    }

    public async Task<MenuDto> UpdateAsync(int id, string name, List<MenuItemInput> items)
    {
        var menu = await db.Menus.Include(m => m.Items).FirstAsync(m => m.Id == id);
        menu.Name = name;
        db.MenuItems.RemoveRange(menu.Items);
        menu.Items = items.Select(i => new MenuItem
        {
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            UnitPrice = i.UnitPrice,
            InitialQuantity = i.Quantity,
            RemainingQuantity = i.Quantity,
        }).ToList();
        await db.SaveChangesAsync();
        return ToDto(menu);
    }

    public async Task DeleteAsync(int id)
    {
        var menu = await db.Menus.FirstAsync(m => m.Id == id);
        db.Menus.Remove(menu);
        await db.SaveChangesAsync();
    }

    public async Task<MenuDto> ActivateAsync(int id)
    {
        var menu = await db.Menus.Include(m => m.Items).FirstAsync(m => m.Id == id);
        await db.Menus
            .Where(m => m.IsActive && m.Type == menu.Type)
            .ExecuteUpdateAsync(s => s.SetProperty(m => m.IsActive, false));
        menu.IsActive = true;
        await db.SaveChangesAsync();
        return ToDto(menu);
    }

    public async Task DeactivateAsync(int id)
    {
        var menu = await db.Menus.FirstAsync(m => m.Id == id);
        menu.IsActive = false;
        await db.SaveChangesAsync();
    }

    public async Task<MenuDto> UpdateItemQuantityAsync(int menuId, int itemId, int newQuantity)
    {
        var menu = await db.Menus.Include(m => m.Items).FirstAsync(m => m.Id == menuId);
        var item = menu.Items.First(i => i.Id == itemId);
        item.RemainingQuantity = Math.Max(0, newQuantity);
        await db.SaveChangesAsync();
        return ToDto(menu);
    }
}
