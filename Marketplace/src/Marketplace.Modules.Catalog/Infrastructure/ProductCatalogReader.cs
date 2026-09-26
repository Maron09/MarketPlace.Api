using Marketplace.Infrastructure.Shared.Persistence;
using Marketplace.Modules.Catalog.Contracts;
using Marketplace.Modules.Catalog.Domain;
using Microsoft.EntityFrameworkCore;



namespace Marketplace.Modules.Catalog.Infrastructure;

internal sealed class ProductCatalogReader : IProductCatalogReader
{
    private readonly MarketplaceDbContext _dbContext;

    public ProductCatalogReader(MarketplaceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProductSnapShotDto?> GetSnapshotAsync(Guid productId, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Set<Product>()
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
        
        return product is null
            ? null
            : new ProductSnapShotDto(product.Id, product.VendorId, product.Name, product.Price, product.IsActive);
    }
}