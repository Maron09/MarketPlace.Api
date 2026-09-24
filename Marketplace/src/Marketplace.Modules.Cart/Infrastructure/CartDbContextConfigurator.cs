using Marketplace.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Modules.Cart.Infrastructure;

internal sealed class CartDbContextConfigurator : IModuleDbContextConfigurator
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CartEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CartItemEntityConfiguration());
    }
}