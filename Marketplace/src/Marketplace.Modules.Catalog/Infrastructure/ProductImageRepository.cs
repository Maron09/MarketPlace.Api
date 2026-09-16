using Marketplace.Infrastructure.Shared.Persistence;
using Marketplace.Modules.Catalog.Application;
using Marketplace.Modules.Catalog.Domain;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Modules.Catalog.Infrastructure
{
    internal sealed class ProductImageRepository : IProductImageRepository
    {
        private readonly MarketplaceDbContext _dbContext;

        public ProductImageRepository(MarketplaceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> GetNextDisplayOrderAsync(Guid productId, CancellationToken cancellationToken)
        {
            var maxOrder = await _dbContext.Set<ProductImage>()
                .Where(pi => pi.ProductId == productId)
                .Select(pi => (int?)pi.DisplayOrder)
                .MaxAsync(cancellationToken);

            return (maxOrder ?? -1) + 1;
        }

        public async Task AddAsync(ProductImage image, CancellationToken cancellationToken)
            => await _dbContext.Set<ProductImage>().AddAsync(image, cancellationToken);

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
            => await _dbContext.SaveChangesAsync(cancellationToken);
    }
}