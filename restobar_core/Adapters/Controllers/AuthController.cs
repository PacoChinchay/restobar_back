using Microsoft.AspNetCore.Mvc;
using restobar_core.Application.DTOs;
using restobar_core.Application.Services;
using restobar_core.Application.UseCases;

namespace restobar_core.Adapters.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthenticateUserUseCase useCase, JwtService jwt) : ControllerBase
{
    [HttpGet("users")]
    public async Task<ActionResult<List<UserDto>>> GetUsers()
    {
        return Ok(await useCase.GetUsersAsync());
    }

    [HttpPost("validate-pin")]
    public async Task<ActionResult<AuthResponse>> ValidatePin([FromBody] ValidatePinRequest request)
    {
        var user = await useCase.ValidatePinAsync(request.UserId, request.Pin);
        if (user is null) return Unauthorized();
        return Ok(new AuthResponse(user, jwt.GenerateToken(user)));
    }
}
