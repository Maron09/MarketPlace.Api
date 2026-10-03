using Marketplace.Modules.Catalog.Contracts;
using Marketplace.Modules.Inventory.Application;
using Marketplace.Modules.Inventory.Contracts;
using Marketplace.Modules.Inventory.Infrastructure;
using Marketplace.SharedKernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Marketplace.Modules.Inventory;

public static class InventoryModuleExtensions
{
    public static IServiceCollection AddInventoryModule(this IServiceCollection services)
    {
        services.AddSingleton<IModuleDbContextConfigurator, InventoryDbContextConfigurator>();
        services.AddScoped<IStockItemRepository, StockItemRepository>();
        services.AddScoped<IStockReservation, StockReservation>();
        services.AddScoped<IStockRelease, StockReservation>();
        services.AddScoped<IStockAvailabilityReader, StockReservation>();

        services.AddScoped(sp => new InventoryApplicationService(
            sp.GetRequiredService<IStockItemRepository>(),
            sp.GetRequiredService<IProductCatalogReader>(),
            sp.GetRequiredService<ILogger<InventoryApplicationService>>()));

        return services;
    }
}