using Microsoft.EntityFrameworkCore;
using restobar_core.Domain.Entities;
using restobar_core.Domain.Ports;

namespace restobar_core.Infrastructure.Persistence.Repositories;

public class SalaryAdvanceRepository(AppDbContext db) : ISalaryAdvanceRepository
{
    public Task<List<SalaryAdvance>> GetByWeekAsync(DateOnly weekStart, DateOnly weekEnd) =>
        db.SalaryAdvances
            .Where(a => a.Date >= weekStart && a.Date <= weekEnd)
            .OrderBy(a => a.CreatedAt)
            .ToListAsync();

    public Task<List<SalaryAdvance>> GetHistoryAsync(string? userId, DateOnly? from, DateOnly? to)
    {
        var q = db.SalaryAdvances.AsQueryable();
        if (userId is not null) q = q.Where(a => a.UserId == userId);
        if (from is not null)   q = q.Where(a => a.Date >= from);
        if (to is not null)     q = q.Where(a => a.Date <= to);
        return q.OrderByDescending(a => a.CreatedAt).ToListAsync();
    }

    public async Task<SalaryAdvance> CreateAsync(SalaryAdvance advance)
    {
        db.SalaryAdvances.Add(advance);

        var session = await db.CashSessions.FirstOrDefaultAsync(s => s.Date == advance.Date);
        if (session is not null)
        {
            db.CashMovements.Add(new CashMovement
            {
                CashSessionId = session.Id,
                MovementType  = "egreso",
                Amount        = advance.Amount,
                Description   = $"Adelanto · {advance.EmployeeName}",
                CreatedAt     = advance.CreatedAt,
                CreatedBy     = advance.RegisteredBy,
            });
        }

        await db.SaveChangesAsync();
        return advance;
    }
}
