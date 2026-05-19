using Microsoft.AspNetCore.Mvc;
using restobar_core.Application.DTOs;
using restobar_core.Application.UseCases;

namespace restobar_core.Adapters.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesController(
    RegisterSaleUseCase registerSaleUseCase,
    GetDailySummaryUseCase getDailySummaryUseCase) : ControllerBase
{
    private static readonly TimeZoneInfo LimaZone =
        TimeZoneInfo.FindSystemTimeZoneById("America/Lima");

    [HttpPost]
    public async Task<ActionResult<SaleDto>> RegisterSale([FromBody] RegisterSaleRequest request)
    {
        var result = await registerSaleUseCase.ExecuteAsync(request);
        return CreatedAtAction(nameof(RegisterSale), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<List<SaleDto>>> GetByDate([FromQuery] DateOnly date)
    {
        var sales = await getDailySummaryUseCase.GetSalesByDateAsync(date);
        return Ok(sales);
    }

    [HttpGet("summary")]
    public async Task<ActionResult<DailySummaryDto>> GetSummary([FromQuery] DateOnly? date)
    {
        var targetDate = date ?? DateOnly.FromDateTime(
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, LimaZone));
        return Ok(await getDailySummaryUseCase.ExecuteAsync(targetDate));
    }

    [HttpGet("weekly")]
    public async Task<ActionResult<List<DailyTotalDto>>> GetWeekly([FromQuery] DateOnly? endDate)
    {
        var targetDate = endDate ?? DateOnly.FromDateTime(
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, LimaZone));
        return Ok(await getDailySummaryUseCase.GetWeeklyTotalsAsync(targetDate));
    }
}
