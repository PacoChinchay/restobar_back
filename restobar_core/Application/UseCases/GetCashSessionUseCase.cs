using restobar_core.Application.DTOs;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class GetCashSessionUseCase(ICashSessionRepository repo)
{
    private static readonly TimeZoneInfo LimaZone =
        TimeZoneInfo.FindSystemTimeZoneById("America/Lima");

    public Task<CashSessionDto?> ExecuteAsync(DateOnly? date = null)
    {
        var d = date ?? DateOnly.FromDateTime(
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, LimaZone));
        return repo.GetByDateAsync(d);
    }
}
