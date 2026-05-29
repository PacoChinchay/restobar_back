using restobar_core.Application.DTOs;
using restobar_core.Domain.Entities;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class OpenCashSessionUseCase(ICashSessionRepository repo)
{
    private static readonly TimeZoneInfo LimaZone =
        TimeZoneInfo.FindSystemTimeZoneById("America/Lima");

    public Task<CashSession> ExecuteAsync(OpenCashSessionRequest req)
    {
        var now = DateTime.UtcNow;
        return repo.OpenAsync(new CashSession
        {
            Date = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(now, LimaZone)),
            InitialAmount = req.InitialAmount,
            OpenedAt = now,
            OpenedBy = req.OpenedBy,
        });
    }
}
