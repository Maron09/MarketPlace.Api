using Marketplace.Modules.Cart.Application;
using Marketplace.Modules.Cart.Infrastructure;
using Marketplace.Modules.Catalog.Contracts;
using Marketplace.SharedKernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace Marketplace.Modules.Cart;

public static class CartModuleExtension
{
    public static IServiceCollection AddCartModule(this IServiceCollection services)
    {
        services.AddSingleton<IModuleDbContextConfigurator, CartDbContextConfigurator>();

        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped(sp => new CartApplicationService(
            sp.GetRequiredService<ICartRepository>(),
            sp.GetRequiredService<IProductCatalogReader>(),
            sp.GetRequiredService<ILogger<CartApplicationService>>()
        ));
        return services;
    }
}