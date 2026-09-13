using Marketplace.Modules.Catalog.Application;
using Marketplace.Modules.Catalog.Infrastructure;
using Marketplace.SharedKernel;
using Microsoft.Extensions.DependencyInjection;


namespace Marketplace.Modules.Catalog
{
    public static class CatalogModuleExtensions
    {
        public static IServiceCollection AddCatalogModule(this IServiceCollection services)
        {
            services.AddSingleton<IModuleDbContextConfigurator, CatalogDbContextConfigurator>();

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped(sp => new CatalogApplicationService(
                sp.GetRequiredService<IProductRepository>()
            ));

            return services;
        }
    }
}