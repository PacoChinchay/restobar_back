using restobar_core.Application.DTOs;
using restobar_core.Domain.Entities;

namespace restobar_core.Domain.Ports;

public interface ICashSessionRepository
{
    Task<CashSessionDto?> GetByDateAsync(DateOnly date);
    Task<CashSession> OpenAsync(CashSession session);
    Task<CashMovement> AddMovementAsync(CashMovement movement);
}
