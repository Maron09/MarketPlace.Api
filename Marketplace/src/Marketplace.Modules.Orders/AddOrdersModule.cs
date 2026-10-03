using Marketplace.Modules.Cart.Contracts;
using Marketplace.Modules.Catalog.Contracts;
using Marketplace.Modules.Inventory.Contracts;
using Marketplace.Modules.Orders.Application;
using Marketplace.Modules.Orders.Infrastructure;
using Marketplace.SharedKernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace Marketplace.Modules.Orders;

public static class OrderModuleExtension
{
    public static IServiceCollection AddOrdersModule(this IServiceCollection services)
    {
        services.AddSingleton<IModuleDbContextConfigurator, OrdersDbContextConfigurator>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IIdempotencyKeyRepository, IdempotencyKeyRepository>();
        services.AddScoped(sp => new OrderApplicationService(
            sp.GetRequiredService<IOrderRepository>(),
            sp.GetRequiredService<ICartReader>(),
            sp.GetRequiredService<IProductCatalogReader>(),
            sp.GetRequiredService<IStockReservation>(),
            sp.GetRequiredService<IStockRelease>(),
            sp.GetRequiredService<IIdempotencyKeyRepository>(),
            sp.GetRequiredService<ILogger<OrderApplicationService>>()
        ));

        return services;
    }
}