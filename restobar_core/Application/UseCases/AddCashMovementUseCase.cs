using restobar_core.Application.DTOs;
using restobar_core.Domain.Entities;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class AddCashMovementUseCase(ICashSessionRepository repo)
{
    public Task<CashMovement> ExecuteAsync(int sessionId, AddCashMovementRequest req)
    {
        return repo.AddMovementAsync(new CashMovement
        {
            CashSessionId = sessionId,
            MovementType = req.MovementType,
            Amount = req.Amount,
            Description = req.Description,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = req.CreatedBy,
        });
    }
}
