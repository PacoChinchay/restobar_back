using Microsoft.EntityFrameworkCore;
using restobar_core.Application.DTOs;
using restobar_core.Domain.Entities;
using restobar_core.Domain.Enums;
using restobar_core.Domain.Ports;
using restobar_core.Infrastructure.Persistence;

namespace restobar_core.Infrastructure.Persistence.Repositories;

public class SaleRepository(AppDbContext db) : ISaleRepository
{
    private static readonly TimeZoneInfo LimaZone =
        TimeZoneInfo.FindSystemTimeZoneById("America/Lima");

    public async Task<Sale> SaveAsync(Sale sale)
    {
        db.Sales.Add(sale);
        await db.SaveChangesAsync();
        return sale;
    }

    public async Task<List<Sale>> GetByDateAsync(DateOnly date)
    {
        // Convert the requested date's midnight→midnight window in Lima time to UTC,
        // then use plain range comparisons that EF Core can always translate to SQL.
        var startLima = date.ToDateTime(TimeOnly.MinValue);
        var startUtc = TimeZoneInfo.ConvertTimeToUtc(startLima, LimaZone);
        var endUtc = startUtc.AddDays(1);

        return await db.Sales
            .Where(s => s.RegisteredAt >= startUtc && s.RegisteredAt < endUtc)
            .OrderByDescending(s => s.RegisteredAt)
            .ToListAsync();
    }

    public async Task<DailySummaryDto> GetDailySummaryAsync(DateOnly date)
    {
        var sales = await GetByDateAsync(date);

        var totalAmount = sales.Sum(s => s.Total);
        var totalSales = sales.Count;

        var grouped = sales
            .GroupBy(s => s.PaymentMethod)
            .ToDictionary(g => g.Key, g => g.Sum(s => s.Total));

        var byPaymentMethod = new ByPaymentMethodDto
        {
            Efectivo = grouped.GetValueOrDefault(PaymentMethod.efectivo, 0),
            Yape = grouped.GetValueOrDefault(PaymentMethod.yape, 0),
            Plin = grouped.GetValueOrDefault(PaymentMethod.plin, 0)
        };

        var recentSales = sales
            .Take(5)
            .Select(s => new SaleDto
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
            })
            .ToList();

        return new DailySummaryDto
        {
            TotalAmount = totalAmount,
            TotalSales = totalSales,
            ByPaymentMethod = byPaymentMethod,
            RecentSales = recentSales
        };
    }
}
