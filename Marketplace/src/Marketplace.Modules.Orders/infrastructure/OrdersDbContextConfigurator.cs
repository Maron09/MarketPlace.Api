using Marketplace.SharedKernel;
using Microsoft.EntityFrameworkCore;


namespace Marketplace.Modules.Orders.Infrastructure;

internal sealed class OrdersDbContextConfigurator : IModuleDbContextConfigurator
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderEntityConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemEntityConfiguration());
        modelBuilder.ApplyConfiguration(new IdempotencyKeyEntityConfiguration());
    }
}