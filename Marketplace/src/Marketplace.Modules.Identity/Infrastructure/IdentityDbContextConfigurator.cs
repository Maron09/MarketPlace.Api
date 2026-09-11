using Marketplace.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Modules.Identity.Infrastructure
{
    internal sealed class IdentityDbContextConfigurator : IModuleDbContextConfigurator
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        }
    }
}