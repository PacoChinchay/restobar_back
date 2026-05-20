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
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IMenuRepository, MenuRepository>();

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
        services.AddScoped<CreateUserUseCase>();
        services.AddScoped<UpdateUserUseCase>();
        services.AddScoped<DeleteUserUseCase>();
        services.AddScoped<GetOpenOrdersUseCase>();
        services.AddScoped<GetOrderUseCase>();
        services.AddScoped<CreateOrderUseCase>();
        services.AddScoped<AddOrderItemUseCase>();
        services.AddScoped<RemoveOrderItemUseCase>();
        services.AddScoped<UpdateOrderItemUseCase>();
        services.AddScoped<PayOrderUseCase>();
        services.AddScoped<CancelOrderUseCase>();
        services.AddScoped<GetMenusUseCase>();
        services.AddScoped<GetActiveMenuUseCase>();
        services.AddScoped<CreateMenuUseCase>();
        services.AddScoped<UpdateMenuUseCase>();
        services.AddScoped<DeleteMenuUseCase>();
        services.AddScoped<ActivateMenuUseCase>();
        services.AddScoped<DeactivateMenuUseCase>();
        services.AddScoped<UpdateMenuItemQuantityUseCase>();

        return services;
    }
}
