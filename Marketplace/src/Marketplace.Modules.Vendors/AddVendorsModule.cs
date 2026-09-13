using Marketplace.Modules.Vendors.Application;
using Marketplace.Modules.Vendors.Infrastructure;
using Marketplace.SharedKernel;
using Microsoft.Extensions.DependencyInjection;


namespace Marketplace.Modules.Vendors
{
    public static class VendorModuleExtensions
    {
        public static IServiceCollection AddVendorsModule(this IServiceCollection services)
        {
            services.AddSingleton<IModuleDbContextConfigurator, VendorsDbContextConfigurator>();
            
            services.AddScoped<IVendorRepository, VendorRepository>();
            services.AddScoped(sp => new VendorApplicationService(
                sp.GetRequiredService<IVendorRepository>()));

            return services;

        }
    }
}
