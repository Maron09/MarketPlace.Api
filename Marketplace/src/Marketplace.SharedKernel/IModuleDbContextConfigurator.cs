using Microsoft.EntityFrameworkCore;

namespace Marketplace.SharedKernel
{
    public interface IModuleDbContextConfigurator
    {
        void Configure(ModelBuilder modelBuilder);
    }
}