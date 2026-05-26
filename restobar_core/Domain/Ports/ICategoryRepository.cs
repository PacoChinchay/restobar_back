using restobar_core.Domain.Entities;

namespace restobar_core.Domain.Ports;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
    Task<Category> CreateAsync(string name, bool isDrink);
    Task<Category> UpdateAsync(int id, string name, bool isDrink);
    Task DeleteAsync(int id);
    Task<bool> IsInUseAsync(int id);
}
