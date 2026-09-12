using Marketplace.Modules.Identity.Infrastructure;
using Marketplace.SharedKernel;
using Microsoft.Extensions.DependencyInjection;
using Marketplace.Modules.Identity.Application;
using Marketplace.Modules.Identity.Domain;
using Microsoft.Extensions.Configuration;


namespace Marketplace.Modules.Identity
{
    public static class IdentityModuleExtensions
    {
        public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

            services.AddSingleton<IModuleDbContextConfigurator, IdentityDbContextConfigurator>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<ITokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped(sp => new RegisterUserService(
                sp.GetRequiredService<IUserRepository>(),
                sp.GetRequiredService<IPasswordHasher>()));

            services.AddScoped(sp => new LoginService(
                sp.GetRequiredService<IUserRepository>(),
                sp.GetRequiredService<IPasswordHasher>(),
                sp.GetRequiredService<ITokenGenerator>()));

            return services;
        }
    }
}