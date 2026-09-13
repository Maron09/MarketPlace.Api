using Marketplace.Modules.Catalog.Domain;

namespace Marketplace.Modules.Catalog.Application
{
    internal interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken);
        Task<IReadOnlyList<Product>> GetActiveAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<IReadOnlyList<Product>> GetByVendorIdAsync(Guid vendorId, int page, int pageSize, CancellationToken cancellationToken);
        Task AddAsync(Product product, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}