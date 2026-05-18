using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class DeleteProductUseCase(IProductRepository repository)
{
    public Task ExecuteAsync(int id) => repository.DeleteAsync(id);
}
