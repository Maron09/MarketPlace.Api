using Microsoft.EntityFrameworkCore;
using Marketplace.SharedKernel;

namespace Marketplace.Infrastructure.Shared.Persistence
{
    public sealed class MarketplaceDbContext : DbContext
    {
        private readonly IEnumerable<IModuleDbContextConfigurator> _configurators;

        public MarketplaceDbContext(
            DbContextOptions<MarketplaceDbContext> options,
            IEnumerable<IModuleDbContextConfigurator> configurators) : base(options)
        {
            _configurators = configurators;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var configurator in _configurators)
            {
                configurator.Configure(modelBuilder);
            }
        }
    }
}