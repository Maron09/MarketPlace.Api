using Marketplace.Modules.Identity.Infrastructure;
using Marketplace.SharedKernel;
using Microsoft.Extensions.DependencyInjection;


namespace Marketplace.Modules.Identity
{
    public static class IdentityModuleExtensions
    {
        public static IServiceCollection AddIdentityModule(this IServiceCollection services)
        {
            services.AddSingleton<IModuleDbContextConfigurator, IdentityDbContextConfigurator>();

            return services;
        }
    }
}