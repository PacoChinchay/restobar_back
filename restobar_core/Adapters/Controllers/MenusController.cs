using Microsoft.AspNetCore.Mvc;
using restobar_core.Application.DTOs;
using restobar_core.Application.UseCases;

namespace restobar_core.Adapters.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenusController(
    GetMenusUseCase getMenus,
    GetActiveMenuUseCase getActiveMenu,
    CreateMenuUseCase createMenu,
    UpdateMenuUseCase updateMenu,
    DeleteMenuUseCase deleteMenu,
    ActivateMenuUseCase activateMenu,
    DeactivateMenuUseCase deactivateMenu,
    UpdateMenuItemQuantityUseCase updateItemQuantity) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<MenuDto>>> GetAll() =>
        Ok(await getMenus.ExecuteAsync());

    [HttpGet("active")]
    public async Task<ActionResult<MenuDto>> GetActive()
    {
        var result = await getActiveMenu.ExecuteAsync();
        return result is null ? NoContent() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<MenuDto>> Create([FromBody] CreateMenuRequest request) =>
        Ok(await createMenu.ExecuteAsync(request));

    [HttpPut("{id}")]
    public async Task<ActionResult<MenuDto>> Update(int id, [FromBody] CreateMenuRequest request) =>
        Ok(await updateMenu.ExecuteAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await deleteMenu.ExecuteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/activate")]
    public async Task<ActionResult<MenuDto>> Activate(int id) =>
        Ok(await activateMenu.ExecuteAsync(id));

    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(int id)
    {
        await deactivateMenu.ExecuteAsync(id);
        return NoContent();
    }

    [HttpPatch("{id}/items/{itemId}/quantity")]
    public async Task<ActionResult<MenuDto>> UpdateItemQuantity(
        int id, int itemId, [FromBody] UpdateMenuItemQuantityRequest request) =>
        Ok(await updateItemQuantity.ExecuteAsync(id, itemId, request.Quantity));
}
