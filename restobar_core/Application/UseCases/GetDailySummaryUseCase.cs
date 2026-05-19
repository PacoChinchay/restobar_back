using restobar_core.Application.DTOs;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class GetDailySummaryUseCase(ISaleRepository repository)
{
    public async Task<List<SaleDto>> GetSalesByDateAsync(DateOnly date)
    {
        var sales = await repository.GetByDateAsync(date);
        return sales.Select(s => new SaleDto
        {
            Id = s.Id,
            ProductId = s.ProductId,
            ProductName = s.ProductName,
            UnitPrice = s.UnitPrice,
            Quantity = s.Quantity,
            PaymentMethod = s.PaymentMethod.ToString(),
            Total = s.Total,
            RegisteredAt = new DateTimeOffset(s.RegisteredAt, TimeSpan.Zero),
            RegisteredBy = s.RegisteredBy
        }).ToList();
    }

    public async Task<DailySummaryDto> ExecuteAsync(DateOnly date)
    {
        return await repository.GetDailySummaryAsync(date);
    }

    public async Task<List<DailyTotalDto>> GetWeeklyTotalsAsync(DateOnly endDate)
    {
        return await repository.GetWeeklyTotalsAsync(endDate);
    }
}
