using Microsoft.AspNetCore.Mvc;
using restobar_core.Application.DTOs;
using restobar_core.Application.UseCases;

namespace restobar_core.Adapters.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(
    AuthenticateUserUseCase getUsers,
    CreateUserUseCase createUser,
    UpdateUserUseCase updateUser,
    DeleteUserUseCase deleteUser) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll() =>
        Ok(await getUsers.GetUsersAsync());

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserRequest request)
    {
        var result = await createUser.ExecuteAsync(request);
        return CreatedAtAction(nameof(GetAll), result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> Update(string id, [FromBody] UpdateUserRequest request) =>
        Ok(await updateUser.ExecuteAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await deleteUser.ExecuteAsync(id);
        return NoContent();
    }
}
