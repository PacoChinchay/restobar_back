using Microsoft.EntityFrameworkCore;
using restobar_core.Application.DTOs;
using restobar_core.Domain.Entities;
using restobar_core.Domain.Ports;

namespace restobar_core.Infrastructure.Persistence.Repositories;

public class CashSessionRepository(AppDbContext db) : ICashSessionRepository
{
    public async Task<CashSessionDto?> GetByDateAsync(DateOnly date)
    {
        var session = await db.CashSessions
            .Include(s => s.Movements.OrderBy(m => m.CreatedAt))
            .FirstOrDefaultAsync(s => s.Date == date);

        if (session is null) return null;

        var balance = session.InitialAmount
            + session.Movements.Where(m => m.MovementType == "ingreso").Sum(m => m.Amount)
            - session.Movements.Where(m => m.MovementType == "egreso").Sum(m => m.Amount);

        return new CashSessionDto(
            session.Id,
            session.Date,
            session.InitialAmount,
            session.OpenedBy,
            session.OpenedAt,
            session.Movements.Select(m =>
                new CashMovementDto(m.Id, m.MovementType, m.Amount, m.Description, m.CreatedAt, m.CreatedBy)
            ).ToList(),
            balance
        );
    }

    public async Task<CashSession> OpenAsync(CashSession session)
    {
        db.CashSessions.Add(session);
        await db.SaveChangesAsync();
        return session;
    }

    public async Task<CashMovement> AddMovementAsync(CashMovement movement)
    {
        db.CashMovements.Add(movement);
        await db.SaveChangesAsync();
        return movement;
    }
}
