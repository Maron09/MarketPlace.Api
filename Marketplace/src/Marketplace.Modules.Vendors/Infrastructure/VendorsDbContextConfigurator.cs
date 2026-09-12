using Marketplace.SharedKernel;
using Microsoft.EntityFrameworkCore;


namespace Marketplace.Modules.Vendors.Infrastructure
{
    internal sealed class VendorsDbContextConfigurator : IModuleDbContextConfigurator
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new VendorEntityConfiguration());
        }
    }
}