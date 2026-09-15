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
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();

            services.AddScoped(sp => new CatalogApplicationService(
                sp.GetRequiredService<IProductRepository>()
            ));

            services.AddScoped(sp => new CategoryApplicationService(
                sp.GetRequiredService<ICategoryRepository>(),
                sp.GetRequiredService<IProductCategoryRepository>(),
                sp.GetRequiredService<IProductRepository>()
            ));

            return services;
        }
    }
}