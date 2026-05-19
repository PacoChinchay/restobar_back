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
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        services.AddScoped<GetProductsUseCase>();
        services.AddScoped<CreateProductUseCase>();
        services.AddScoped<UpdateProductUseCase>();
        services.AddScoped<DeleteProductUseCase>();
        services.AddScoped<RegisterSaleUseCase>();
        services.AddScoped<GetDailySummaryUseCase>();
        services.AddScoped<AuthenticateUserUseCase>();
        services.AddScoped<GetCategoriesUseCase>();
        services.AddScoped<CreateCategoryUseCase>();
        services.AddScoped<UpdateCategoryUseCase>();
        services.AddScoped<DeleteCategoryUseCase>();

        return services;
    }
}
