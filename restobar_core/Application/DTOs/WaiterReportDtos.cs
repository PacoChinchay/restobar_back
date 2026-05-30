namespace restobar_core.Application.DTOs;

public record WaiterWeekSummaryDto(
    List<WaiterStatsDto> Waiters,
    List<DailyOrderCountDto> DailyTotals
);

public record WaiterStatsDto(
    string Name,
    int TotalOrders,
    int TotalTables,
    decimal TotalRevenue,
    decimal AvgOrderValue
);

public record DailyOrderCountDto(string Date, int OrderCount);

public record WaiterDaySummaryDto(string Date, List<WaiterStatsDto> Waiters);
