using Marketplace.Infrastructure.Shared.Persistence;
using Marketplace.Modules.Catalog.Application;
using Marketplace.Modules.Catalog.Domain;
using Microsoft.EntityFrameworkCore;


namespace Marketplace.Modules.Catalog.Infrastructure
{
    internal sealed class ProductRepository : IProductRepository
    {
        private readonly MarketplaceDbContext _dbContext;

        public ProductRepository(MarketplaceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken)
            => await _dbContext.Set<Product>().FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
        
        public async Task<IReadOnlyList<Product>> GetActiveAsync(int page, int pageSize, CancellationToken cancellationToken)
            => await _dbContext.Set<Product>()
                .Where(p => p.IsActive)
                .OrderBy(p => p.CreatedAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        
        public async Task<IReadOnlyList<Product>> GetByVendorIdAsync(Guid vendorId, int page, int pageSize, CancellationToken cancellationToken)
            => await _dbContext.Set<Product>()
                .Where(p => p.VendorId == vendorId)
                .OrderBy(p => p.CreatedAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        
        public async Task AddAsync(Product product, CancellationToken cancellationToken)
            => await _dbContext.Set<Product>().AddAsync(product, cancellationToken);
        
        public async Task SaveChangesAsync(CancellationToken cancellationToken)
            => await _dbContext.SaveChangesAsync(cancellationToken);
    }
}