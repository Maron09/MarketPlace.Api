using Marketplace.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Modules.Catalog.Infrastructure
{
    internal sealed class CatalogDbContextConfigurator : IModuleDbContextConfigurator
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ProductCategoryEntityConfiguration());
        }
    }
}