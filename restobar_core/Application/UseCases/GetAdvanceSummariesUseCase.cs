using restobar_core.Application.DTOs;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class GetAdvanceSummariesUseCase(IAuthPort authPort, ISalaryAdvanceRepository advRepo)
{
    private static readonly TimeZoneInfo LimaZone =
        TimeZoneInfo.FindSystemTimeZoneById("America/Lima");

    public async Task<List<EmployeeAdvanceSummaryDto>> ExecuteAsync(DateOnly? weekOf = null)
    {
        var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, LimaZone));
        var pivot = weekOf ?? today;
        var daysFromMonday = ((int)pivot.DayOfWeek + 6) % 7;
        var weekStart = pivot.AddDays(-daysFromMonday);
        var weekEnd   = weekStart.AddDays(6);

        var users    = await authPort.GetUsersAsync();
        var advances = await advRepo.GetByWeekAsync(weekStart, weekEnd);

        return users
            .Select(u =>
            {
                var userAdvs   = advances.Where(a => a.UserId == u.Id).ToList();
                var weekly     = (u.MonthlySalary ?? 0m) / 4m;
                var advancedWk = userAdvs.Sum(a => a.Amount);
                var advDtos    = userAdvs
                    .Select(a => new SalaryAdvanceDto(a.Id, a.UserId, a.EmployeeName, a.Amount, a.Date, a.Notes, a.RegisteredBy, a.CreatedAt))
                    .ToList();
                return new EmployeeAdvanceSummaryDto(
                    u.Id, u.Name, u.Role.ToString(),
                    u.MonthlySalary ?? 0m, weekly, advancedWk, weekly - advancedWk,
                    advDtos);
            })
            .OrderBy(e => e.Name)
            .ToList();
    }
}
