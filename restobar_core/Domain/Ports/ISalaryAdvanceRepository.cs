using restobar_core.Domain.Entities;

namespace restobar_core.Domain.Ports;

public interface ISalaryAdvanceRepository
{
    Task<List<SalaryAdvance>> GetByWeekAsync(DateOnly weekStart, DateOnly weekEnd);
    Task<List<SalaryAdvance>> GetHistoryAsync(string? userId, DateOnly? from, DateOnly? to);
    Task<SalaryAdvance> CreateAsync(SalaryAdvance advance);
}
