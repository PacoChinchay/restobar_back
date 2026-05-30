using restobar_core.Application.DTOs;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class GetWaiterReportUseCase(IOrderRepository repo)
{
    public Task<WaiterWeekSummaryDto> GetWeekStatsAsync(DateOnly endDate) =>
        repo.GetWaiterWeekStatsAsync(endDate);

    public Task<WaiterDaySummaryDto> GetDayStatsAsync(DateOnly date) =>
        repo.GetWaiterDayStatsAsync(date);
}
