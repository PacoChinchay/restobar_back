using restobar_core.Domain.Entities;

namespace restobar_core.Domain.Ports;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
    Task<Category> CreateAsync(string name);
    Task<Category> UpdateAsync(int id, string name);
    Task DeleteAsync(int id);
    Task<bool> IsInUseAsync(int id);
}
