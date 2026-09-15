using Marketplace.Infrastructure.Shared.Persistence;
using Marketplace.Modules.Catalog.Application;
using Marketplace.Modules.Catalog.Domain;
using Microsoft.EntityFrameworkCore;


namespace Marketplace.Modules.Catalog.Infrastructure
{
    internal sealed class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly MarketplaceDbContext _dbContext;

        public ProductCategoryRepository(MarketplaceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ExistsAsync(Guid productId, Guid categoryId, CancellationToken cancellationToken)
            => await _dbContext.Set<ProductCategory>().AnyAsync(pc => pc.ProductId == productId && pc.CategoryId == categoryId, cancellationToken);
        
        public async Task AddAsync(Guid productId, Guid categoryId, CancellationToken cancellationToken)
            => await _dbContext.Set<ProductCategory>().AddAsync(new ProductCategory(productId, categoryId), cancellationToken);
        
        public async Task RemoveAsync(Guid productId, Guid categoryId, CancellationToken cancellationToken)
        {
            var link = await _dbContext.Set<ProductCategory>()
                .FirstOrDefaultAsync(pc => pc.ProductId == productId && pc.CategoryId == categoryId, cancellationToken);
            if (link is not null)
                _dbContext.Set<ProductCategory>().Remove(link);
        }
        
        public async Task SaveChangesAsync(CancellationToken cancellationToken)
            => await _dbContext.SaveChangesAsync(cancellationToken);
    }
}