using restobar_core.Application.UseCases;
using restobar_core.Domain.Ports;
using restobar_core.Infrastructure.Persistence.Repositories;

namespace restobar_core.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<IAuthPort, AuthRepository>();

        services.AddScoped<GetProductsUseCase>();
        services.AddScoped<RegisterSaleUseCase>();
        services.AddScoped<GetDailySummaryUseCase>();
        services.AddScoped<AuthenticateUserUseCase>();

        return services;
    }
}
