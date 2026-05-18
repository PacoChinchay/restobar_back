using restobar_core.Application.DTOs;
using restobar_core.Domain.Entities;
using restobar_core.Domain.Enums;
using restobar_core.Domain.Ports;

namespace restobar_core.Application.UseCases;

public class RegisterSaleUseCase(ISaleRepository repository)
{
    public async Task<SaleDto> ExecuteAsync(RegisterSaleRequest request)
    {
        var sale = new Sale
        {
            ProductId = request.ProductId,
            ProductName = request.ProductName,
            UnitPrice = request.UnitPrice,
            Quantity = request.Quantity,
            PaymentMethod = Enum.Parse<PaymentMethod>(request.PaymentMethod, ignoreCase: true),
            Total = request.Total,
            RegisteredAt = request.RegisteredAt.UtcDateTime,
            RegisteredBy = request.RegisteredBy
        };

        var saved = await repository.SaveAsync(sale);

        return new SaleDto
        {
            Id = saved.Id,
            ProductId = saved.ProductId,
            ProductName = saved.ProductName,
            UnitPrice = saved.UnitPrice,
            Quantity = saved.Quantity,
            PaymentMethod = saved.PaymentMethod.ToString(),
            Total = saved.Total,
            RegisteredAt = new DateTimeOffset(saved.RegisteredAt, TimeSpan.Zero),
            RegisteredBy = saved.RegisteredBy
        };
    }
}
