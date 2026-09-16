using Marketplace.Modules.Catalog.Domain;

namespace Marketplace.Modules.Catalog.Application
{
    internal interface IProductImageRepository
    {
        Task<int> GetNextDisplayOrderAsync(Guid productId, CancellationToken cancellationToken);
        Task AddAsync(ProductImage image, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}