using restobar_core.Application.DTOs;
using restobar_core.Domain.Entities;

namespace restobar_core.Domain.Ports;

public interface ISaleRepository
{
    Task<Sale> SaveAsync(Sale sale);
    Task<List<Sale>> GetByDateAsync(DateOnly date);
    Task<DailySummaryDto> GetDailySummaryAsync(DateOnly date);
}
