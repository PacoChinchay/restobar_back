using Microsoft.AspNetCore.Mvc;
using restobar_core.Application.DTOs;
using restobar_core.Application.UseCases;

namespace restobar_core.Adapters.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController(GetWaiterReportUseCase waiterReport) : ControllerBase
{
    private static readonly TimeZoneInfo LimaZone =
        TimeZoneInfo.FindSystemTimeZoneById("America/Lima");

    [HttpGet("waiters/weekly")]
    public async Task<ActionResult<WaiterWeekSummaryDto>> GetWaitersWeekly([FromQuery] DateOnly? endDate)
    {
        var target = endDate ?? DateOnly.FromDateTime(
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, LimaZone));
        return Ok(await waiterReport.GetWeekStatsAsync(target));
    }
}
