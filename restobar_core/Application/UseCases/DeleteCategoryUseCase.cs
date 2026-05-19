using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class DeleteCategoryUseCase(ICategoryRepository repository)
{
    public async Task ExecuteAsync(int id)
    {
        if (await repository.IsInUseAsync(id))
            throw new InvalidOperationException("No se puede eliminar una categoría que tiene productos asignados.");

        await repository.DeleteAsync(id);
    }
}
