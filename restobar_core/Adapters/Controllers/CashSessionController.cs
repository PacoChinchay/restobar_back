using Microsoft.AspNetCore.Mvc;
using restobar_core.Application.DTOs;
using restobar_core.Application.UseCases;

namespace restobar_core.Adapters.Controllers;

[ApiController]
[Route("api/cash-session")]
public class CashSessionController(
    GetCashSessionUseCase getCashSession,
    OpenCashSessionUseCase openCashSession,
    AddCashMovementUseCase addCashMovement) : ControllerBase
{
    private static readonly TimeZoneInfo LimaZone =
        TimeZoneInfo.FindSystemTimeZoneById("America/Lima");

    [HttpGet("today")]
    public async Task<ActionResult<CashSessionDto?>> GetToday()
    {
        var date = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, LimaZone));
        return Ok(await getCashSession.ExecuteAsync(date));
    }

    [HttpGet]
    public async Task<ActionResult<CashSessionDto?>> GetByDate([FromQuery] DateOnly date)
        => Ok(await getCashSession.ExecuteAsync(date));

    [HttpPost]
    public async Task<ActionResult> Open([FromBody] OpenCashSessionRequest request)
    {
        var session = await openCashSession.ExecuteAsync(request);
        return Ok(new
        {
            session.Id,
            session.Date,
            session.InitialAmount,
            session.OpenedBy,
            session.OpenedAt,
            Movements = new List<object>(),
            Balance = session.InitialAmount,
        });
    }

    [HttpPost("{sessionId:int}/movements")]
    public async Task<ActionResult<CashMovementDto>> AddMovement(
        int sessionId, [FromBody] AddCashMovementRequest request)
    {
        var m = await addCashMovement.ExecuteAsync(sessionId, request);
        return Ok(new CashMovementDto(m.Id, m.MovementType, m.Amount, m.Description, m.CreatedAt, m.CreatedBy));
    }
}
