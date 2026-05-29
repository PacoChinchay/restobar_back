using restobar_core.Application.DTOs;
using restobar_core.Domain.Entities;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class CreateAdvanceUseCase(ISalaryAdvanceRepository advRepo)
{
    private static readonly TimeZoneInfo LimaZone =
        TimeZoneInfo.FindSystemTimeZoneById("America/Lima");

    public Task<SalaryAdvance> ExecuteAsync(CreateAdvanceRequest req)
    {
        var now = DateTime.UtcNow;
        return advRepo.CreateAsync(new SalaryAdvance
        {
            UserId       = req.UserId,
            EmployeeName = req.EmployeeName,
            Amount       = req.Amount,
            Date         = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(now, LimaZone)),
            Notes        = req.Notes,
            RegisteredBy = req.RegisteredBy,
            CreatedAt    = now,
        });
    }
}
