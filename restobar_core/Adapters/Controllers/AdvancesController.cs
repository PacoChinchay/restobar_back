using Microsoft.AspNetCore.Mvc;
using restobar_core.Application.DTOs;
using restobar_core.Application.UseCases;

namespace restobar_core.Adapters.Controllers;

[ApiController]
[Route("api/advances")]
public class AdvancesController(
    GetAdvanceSummariesUseCase getSummaries,
    CreateAdvanceUseCase createAdvance) : ControllerBase
{
    private static readonly TimeZoneInfo LimaZone =
        TimeZoneInfo.FindSystemTimeZoneById("America/Lima");

    [HttpGet("summary")]
    public async Task<ActionResult<List<EmployeeAdvanceSummaryDto>>> GetSummary([FromQuery] DateOnly? weekOf)
        => Ok(await getSummaries.ExecuteAsync(weekOf));

    [HttpPost]
    public async Task<ActionResult<SalaryAdvanceDto>> Create([FromBody] CreateAdvanceRequest request)
    {
        var a = await createAdvance.ExecuteAsync(request);
        return Ok(new SalaryAdvanceDto(a.Id, a.UserId, a.EmployeeName, a.Amount, a.Date, a.Notes, a.RegisteredBy, a.CreatedAt));
    }
}
