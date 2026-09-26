
using Marketplace.SharedKernel;
using Microsoft.EntityFrameworkCore;


namespace Marketplace.Modules.Inventory.Infrastructure;

internal sealed class InventoryDbContextConfigurator : IModuleDbContextConfigurator
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new StockItemEntityConfiguration());
    }
}